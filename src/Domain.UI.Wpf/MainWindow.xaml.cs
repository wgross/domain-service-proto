using Domain.UI.Wpf.ViewModel;
using System.Windows;

namespace Domain.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.InitializeComponent();
            this.Loaded += this.MainWindow_Loaded;
            this.viewModel = viewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.viewModel;
        }
    }
}