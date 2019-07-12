using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace GMapSample {
	public class MainWindowViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(object s, PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(s, e);
		}
		private void SetProperty<T>(ref T member, T value, [CallerMemberName]string propName = null)
		{
			if (member?.Equals(value) ?? false) { return; }
			member = value;
			OnPropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
		private Window _ownner;


		public GMapControl GmapControl {
			get { return _gmapControl; }
			set { SetProperty(ref _gmapControl, value); }
		}

		public ObservableCollection<GMapMarker> Markers {
			get {
				return GmapControl.Markers;
			}
		}

		public WindowClosing ClosingHandler {
			get {
				return (s, e) => {
					Debug.WriteLine("closing test");
				};
			}
		}

		public delegate void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e);

		public MainWindowViewModel(Window ownner)
		{
			_ownner = ownner;
			InitGmap();
			ChangeMapProvider(GMapProviders.GoogleTerrainMap);

			_ownner.Closing += (s, e) => {
				GmapControl.Manager.CancelTileCaching();
				Debug.WriteLine("closing test");
			};
		}

		private void InitGmap()
		{
			GmapControl = new GMapControl();

			GmapControl.Manager.BoostCacheEngine = true;
			GmapControl.Manager.Mode = AccessMode.ServerAndCache;
			GmapControl.CanDragMap = true;
			GmapControl.DragButton = MouseButton.Left;
			GmapControl.Zoom = 2;
			GmapControl.MinZoom = 2;
			GmapControl.MaxZoom = 16;
			GmapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
			GmapControl.ShowCenter = false;

			GmapControl.PreviewMouseLeftButtonDown += GmapControlMouseLeftButtonHandler;
			GmapControl.MouseLeftButtonUp += GmapControlMouseLeftButtonHandler;
		}

		private void GmapControlMouseLeftButtonHandler(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released) 
			{
				GmapControl.CanDragMap = true;
			}
			else if (e.LeftButton == MouseButtonState.Pressed) 
			{
				if (Keyboard.IsKeyDown(Key.M)) 
				{
					GmapControl.CanDragMap = false;

					Point _pt = e.GetPosition(GmapControl);
					PointLatLng _latlon = GmapControl.FromLocalToLatLng((int)_pt.X, (int)_pt.Y);

					Debug.WriteLine($"Marker: {_latlon}");

					var _gm = new GMapMarker(_latlon);
					
					_gm.Shape = new Ellipse()
					{
						Width = 10,
						Height = 10,
						RenderTransform = new TranslateTransform(-5, -5),
						Stroke = Brushes.Red,
						StrokeThickness = 1.5
					};

					GmapControl.Markers.Add(_gm);
				}
				else if (Keyboard.IsKeyDown(Key.P)) 
				{
					GmapControl.CanDragMap = false;

					Point _pt = e.GetPosition(GmapControl);
					PointLatLng _latlon = GmapControl.FromLocalToLatLng((int)_pt.X, (int)_pt.Y);

					if (_gmapRout is null) {
						_gmapRout = new GMapRoute(new List<PointLatLng>());

						GmapControl.Markers.Add(_gmapRout);
					}

					_gmapRout.Points.Add(_latlon);
					_gmapRout.RegenerateShape(GmapControl);

					if (_gmapRout.Shape is null) { return; }
					Path _grPath = _gmapRout.Shape as Path;
					_grPath.Stroke = Brushes.Red;
				}
			}
		}

		private void ChangeMapProvider(GMapProvider provider)
		{
			if(GmapControl is null) { return; }
			GmapControl.MapProvider = provider;
		}

		private GMapControl _gmapControl;
		private GMapRoute _gmapRout;

	}
}
