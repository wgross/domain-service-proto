using domain.model;
using domain.persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace domain.host.Hosting
{
    public class MigrateDatabaseService : IHostedService
    {
        private readonly IServiceProvider services;

        public MigrateDatabaseService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = this.services.CreateScope();
            using var domainModel = (DomainModel)scope.ServiceProvider.GetRequiredService<IDomainModel>();

            await domainModel.Database.EnsureCreatedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}