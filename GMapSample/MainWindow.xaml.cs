using System.Windows;

namespace GMapSample {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window {
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), 
				typeof(MainWindow), new PropertyMetadata(null));

		public MainWindowViewModel ViewModel {
			get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
			private set { SetValue(ViewModelProperty, value); }
		}

		public MainWindow()
		{
			ViewModel = new MainWindowViewModel(this);

			InitializeComponent();
		}
	}
}
