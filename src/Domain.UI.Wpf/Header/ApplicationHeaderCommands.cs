using System.Windows.Input;

namespace Domain.UI.Wpf.Header
{
    public static class ApplicationHeaderCommands
    {
        /// <summary>
        /// Makes the ApplicationHeader Show the Login Dialog
        /// </summary>
        public static RoutedUICommand LoginCommand = new("Login", nameof(LoginCommand), typeof(ApplicationCommands));

        /// <summary>
        /// Makes the ApplicationHedader logout the current user.
        /// </summary>
        public static RoutedUICommand LogoutCommand = new("Logout", nameof(LogoutCommand), typeof(ApplicationCommands));
    }
}