using Domain.Client.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Domain.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddJsonDomainServiceClient(builder.Configuration.GetSection("Endpoints"));
            builder.Services.AddHttpClient("DomainService")
                .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
                .ConfigureHandler(
                    authorizedUrls: new[] { "https://localhost:6001" },
                    scopes: new[] { "DomainService" }));

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Authorization", options.ProviderOptions);
                options.ProviderOptions.DefaultScopes.Add("DomainService");
            });

            await builder.Build().RunAsync();
        }
    }
}