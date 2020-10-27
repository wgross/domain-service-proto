using domain.model;
using domain.persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace domain.persistence.test
{
    public class DomainModelTest : IDisposable
    {
        private readonly string connectionString;
        private readonly ServiceProvider serviceProvider;
        private bool disposedValue;

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

            using var model = NewModel();

            ((DomainModel)model).Database.EnsureCreated();
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using var model = NewModel();

                    ((DomainModel)model).Database.EnsureDeleted();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        private IDomainModel NewModel() => this.serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDomainModel>();

        [Fact]
        public async Task DomainEntityRepository_creates_new_entity()
        {
            // ARRANGE

            var entity = new DomainEntity
            {
                Text = "data"
            };

            // ACT

            using var model = NewModel();

            await model.Entities.Add(entity);

            var result = await model.SaveChanges();

            // ASSERT

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DomainEntityRepository_reads_all_entities()
        {
            // ARRANGE

            var entity = new DomainEntity
            {
                Text = "data"
            };

            using var arrangeModel = NewModel();

            await arrangeModel.Entities.Add(entity);
            await arrangeModel.SaveChanges();

            // ACT

            using var actModel = NewModel();

            var result = await actModel.Entities.Query().ToArrayAsync();

            // ASSERT

            Assert.NotSame(entity, result.Single());
            Assert.Equal(entity.Id, result.Single().Id);
            Assert.Equal(entity.Text, result.Single().Text);
        }

        [Fact]
        public async Task DomainEntityRepository_reads_entitiy_by_id()
        {
            // ARRANGE

            var entity = new DomainEntity
            {
                Text = "data"
            };

            using var arrangeModel = NewModel();

            await arrangeModel.Entities.Add(entity);
            await arrangeModel.SaveChanges();

            // ACT

            using var actModel = NewModel();

            var result = await actModel.Entities.FindById(entity.Id);

            // ASSERT

            Assert.NotSame(entity, result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Text, result.Text);
        }

        [Fact]
        public async Task DomainEntityRepository_reading_entitiy_by_id_returns_null_on_unkown_id()
        {
            // ACT

            using var actModel = NewModel();

            var result = await actModel.Entities.FindById(Guid.NewGuid());

            // ASSERT

            Assert.Null(result);
        }

        [Fact]
        public async Task DomainEntityRepository_updates_entitiy_by_id()
        {
            // ARRANGE

            var arrangeEntity = new DomainEntity
            {
                Text = "data"
            };

            using var arrangeModel = NewModel();

            await arrangeModel.Entities.Add(arrangeEntity);
            await arrangeModel.SaveChanges();

            // ACT

            using var actModel = NewModel();

            var actEntity = await actModel.Entities.FindById(arrangeEntity.Id);

            actEntity.Text = "changed";

            var result = await actModel.SaveChanges();

            // ASSERT

            Assert.Equal(1, result);

            using var assertModel = NewModel();

            var assertEntity = await assertModel.Entities.FindById(arrangeEntity.Id);

            Assert.NotSame(arrangeEntity, assertEntity);
            Assert.Equal(arrangeEntity.Id, assertEntity.Id);
            Assert.Equal("changed", assertEntity.Text);
        }

        [Fact]
        public async Task DomainEntityRepository_deletes_entitiy_by_id()
        {
            // ARRANGE

            var arrangeEntity = new DomainEntity
            {
                Text = "data"
            };

            using var arrangeModel = NewModel();

            await arrangeModel.Entities.Add(arrangeEntity);
            await arrangeModel.SaveChanges();

            // ACT

            using var actModel = NewModel();

            var actEntity = await actModel.Entities.FindById(arrangeEntity.Id);
            actModel.Entities.Delete(actEntity);

            var result = await actModel.SaveChanges();

            // ASSERT

            Assert.Equal(1, result);

            var assertModel = NewModel();

            Assert.Null(await assertModel.Entities.FindById(arrangeEntity.Id));
        }
    }
}