using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Domain.UI.Wpf.ViewModel
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private LoginViewModel loginViewModel;

        public MainWindowViewModel(LoginViewModel loginViewModel)
        {
            this.LoginViewModel = loginViewModel;
        }

        public LoginViewModel LoginViewModel
        {
            get => this.loginViewModel;
            private set => this.SetProperty(ref this.loginViewModel, value);
        }
    }
}