using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Domain.Host.TestServer
{
    /// <summary>
    /// Starts the Domain.Host as an in-process service.
    /// </summary>
    public sealed class DomainHostTestServer : WebApplicationFactory<Domain.Host.Program>
    {
        private readonly Microsoft.AspNetCore.TestHost.TestServer identityTestServer;

        public DomainHostTestServer(Microsoft.AspNetCore.TestHost.TestServer identityServer)
        {
            this.identityTestServer = identityServer;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Microsoft.Extensions.Hosting.Host
               .CreateDefaultBuilder()
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup(this.StartUpFactory);
               });
        }

        /// <summary>
        /// Inject the identity server test instance in to the startup configuration
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Startup StartUpFactory(WebHostBuilderContext arg)
            => new DomainServiceTestHostStartup(arg.HostingEnvironment, arg.Configuration, this.identityTestServer);
    }
}