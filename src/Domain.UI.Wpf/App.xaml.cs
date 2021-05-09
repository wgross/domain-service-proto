using Domain.Client.Authorization;
using Domain.Client.Json;
using Domain.UI.Wpf.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Windows;

namespace Domain.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;
        private IServiceProvider Services => this.host.Services;

        public App()
        {
            this.host = new HostBuilder()
                .ConfigureAppConfiguration(static (context, cfgBuilder) =>
                {
                    cfgBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    cfgBuilder.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<LoginViewModel>();
                    // login of users
                    this.AddAuthentication(services);
                })
                .ConfigureLogging(static logging =>
                {
                    logging.AddConsole();
                })
                .Build();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            ///TODO: AUTH: Authority is hard coded, should com from config
            services.AddSingleton(new DomainUserTokenProvider(new Uri("https://localhost:7777"), new HttpClient()));
            services.AddSingleton<ITokenProvider>(sp => sp.GetRequiredService<DomainUserTokenProvider>());
            services.AddSingleton<IDomainUserTokenProvider>(sp => sp.GetRequiredService<DomainUserTokenProvider>());
        }

        /// <summary>
        /// The Startup event handler replaces the <see cref="Application.StartupUri"/> to fetch the
        /// <see cref="MainWindow"/> from the <see cref="IServiceCollection"/>
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await this.host.StartAsync();
            this.Services.GetService<MainWindow>().Show();
        }

        /// <summary>
        /// Stop and dispose the <see cref="host"/>
        /// </summary>
        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (this.host)
                await this.host.StopAsync();
        }
    }
}