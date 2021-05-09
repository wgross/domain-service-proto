using Domain.Client.Authorization;
using Domain.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Client.Json
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonDomainServiceClient(this IServiceCollection services, IConfiguration configuration = null)
        {
            services.Configure<JsonDomainClientOptions>(configuration.GetSection(JsonDomainClientOptions.SectionName));
            services.AddHttpClient<IDomainService, JsonDomainClient>().AddHttpMessageHandler<DomainClientAuthorizingHandler>();

            return services;
        }
    }
}