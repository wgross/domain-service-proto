using Domain.UI.Wpf.Header.Login;
using Domain.UI.Wpf.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Domain.UI.Wpf.Header
{
    /// <summary>
    /// Interaction logic for ApplicationHeader.xaml
    /// </summary>
    public partial class ApplicationHeader : UserControl
    {
        public ApplicationHeader()
        {
            this.InitializeComponent();
            this.Loaded += this.ApplicationHeader_Loaded;
        }

        private void ApplicationHeader_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CommandBindings.Add(new(ApplicationHeaderCommands.LoginCommand, this.LoginCommandExecuted, this.LoginCommandCanExecute));
        }

        private void LoginCommandExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (this.DataContext is MainWindowViewModel mainViewModel)
            {
                var loginDlg = new LoginDialog
                {
                    DataContext = mainViewModel.LoginViewModel
                };
                var result = loginDlg.ShowDialog();
            }
        }

        private void LoginCommandCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
            args.Handled = true;
        }
    }
}