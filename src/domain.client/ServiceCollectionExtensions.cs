using Domain.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonDomainServiceClient(this IServiceCollection services, IConfigurationSection section)
        {
            services.AddTransient(sp => section.Get<JsonDomainClientOptions>());
            services.AddHttpClient<IDomainService, JsonDomainClient>();
            return services;
        }
    }
}