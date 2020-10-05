using domain.model;
using domain.persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace domain.persistence.test
{
    public class DomainModelTest
    {
        private readonly string connectionString;
        private readonly ServiceProvider serviceProvider;

        public DomainModelTest()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\DomainDatabaseTest.{instanceId}.db";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(DomainModelTest).GetTypeInfo().Assembly.Location));

            this.serviceProvider = new ServiceCollection()
                .AddDbContext<DomainDbContext>(opts => opts.UseSqlite(this.connectionString))
                .AddTransient<IDomainModel, DomainModel>()
                .BuildServiceProvider();
        }

        private IDomainModel NewModel() => this.serviceProvider.GetRequiredService<IDomainModel>();

        public class Observer : IObserver<DomainEvent>
        {
            public List<DomainEvent> Events { get; } = new List<DomainEvent>();

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(DomainEvent value) => this.Events.Add(value);
        }

        [Fact]
        public void DomainModel_sends_event()
        {
            // ARRANGE

            using var model = NewModel();

            var observer = new Observer();

            using var subscription = model.DomainEvents.Subscribe(observer);

            var entity = new DomainEntity();

            // ACT

            model.Entities.Add(entity);

            // ASSERT

            Assert.Single(observer.Events);
            Assert.Equal("Added", observer.Events.Single().Event);
            Assert.Equal(entity, observer.Events.Single().Data);
        }
    }
}