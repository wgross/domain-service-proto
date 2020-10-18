using domain.persistence.EF;
using domain.host;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace domain.host.test
{
    public sealed class DomainServiceTestHost : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\DomainDatabaseTest.{instanceId}.db";

                var connectionString = createConnectionString(
                    instanceId: Guid.NewGuid(),
                    path: Path.GetDirectoryName(typeof(DomainServiceTestHost).GetTypeInfo().Assembly.Location));

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DomainDbContext>));
                if (descriptor is { })
                    services.Remove(descriptor);

                services.AddDbContext<DomainDbContext>(opts => opts.UseSqlite(connectionString));
            });
        }
    }
}