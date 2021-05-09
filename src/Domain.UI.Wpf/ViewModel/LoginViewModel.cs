using Domain.Client.Authorization;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace Domain.UI.Wpf.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IDomainUserTokenProvider tokenProvider;
        private string username;
        private string password;
        private bool isLoggedIn;
        private string loginError;

        public LoginViewModel(IDomainUserTokenProvider tokenProvider)
        {
            this.IsLoggedIn = false;
            this.Username = null;
            this.LoginCommand = new RelayCommand(this.ExecuteLoginCommand);
            this.LogoutCommand = new RelayCommand(this.ExecuteLogoutCommand);
            this.tokenProvider = tokenProvider;
        }

        public string Username
        {
            get => this.username;
            set => this.SetProperty(ref this.username, value);
        }

        public string Password
        {
            get => this.password;
            set => this.SetProperty(ref this.password, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand LogoutCommand { get; }

        public bool IsLoggedIn
        {
            get => this.isLoggedIn;
            set => this.SetProperty(ref this.isLoggedIn, value);
        }

        public string LoginError
        {
            get => this.loginError;
            set => this.SetProperty(ref this.loginError, value);
        }

        private void ExecuteLoginCommand()
        {
            try
            {
                this.tokenProvider.FetchToken(
                    //TODO: AUTH: client is hardcoded. should  com from  config
                    clientId: "wpf", clientSecret: "secret",
                    // user name and password are set interactively from the view.
                    userId: this.Username,
                    userSecret: this.Password,
                    //TODO: scopes might come from a central place or fro config just for aestetics
                    scopes: new[] { "domain-api" }
                );
                this.IsLoggedIn = true;
                this.LoginError = null;
            }
            catch (Exception ex)
            {
                this.IsLoggedIn = false;
                this.LoginError = ex.Message;
                this.Password = null;
            }
        }

        private void ExecuteLogoutCommand()
        {
            this.Username = null;
            this.Password = null;
            this.IsLoggedIn = false;
        }
    }
}