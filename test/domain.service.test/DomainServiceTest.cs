using domain.contract.test;
using domain.model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace domain.service.test
{
    [Collection(nameof(DomainService))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class DomainServiceTest : DomainServiceContractTestBase, IDisposable
    {
        private MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        private Mock<IDomainModel> DomainModelMock { get; }

        private Mock<IDomainEntityRepository> DomainEntityRepositoryMock { get; }

        public DomainServiceTest()
        {
            this.DomainModelMock = this.Mocks.Create<IDomainModel>();
            this.DomainEntityRepositoryMock = this.Mocks.Create<IDomainEntityRepository>();
            this.DomainContract = new DomainService(this.DomainModelMock.Object);
        }

        public void Dispose() => this.Mocks.VerifyAll();

        #region Domain Command Path

        [Fact]
        public Task DomainService_does_something() => base.DomainContract_does_something();

        [Fact]
        public Task DomainService_doing_someting_fails_on_missing_body() => base.DomainContract_doing_someting_fails_on_missing_body();

        [Fact]
        public Task DomainService_doing_something_fails_on_bad_input() => base.DomainContract_doing_something_fails_on_bad_input();

        [Fact]
        public async Task DomainService_creates_entity()
        {
            // ARRANGE

            this.DomainEntityRepositoryMock
                .Setup(r => r.Add(It.IsAny<DomainEntity>()))
                .Callback<DomainEntity>(e => e.Id = Guid.NewGuid())
                .Returns(Task.CompletedTask);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            this.DomainModelMock
                .Setup(m => m.SaveChanges())
                .ReturnsAsync(1);

            // ACT

            await base.DomainContract_creates_entity();
        }

        [Fact]
        public async Task DomainService_creating_entity_fails_on_null_data()
        {
            await this.DomainContract_creating_entity_fails_on_null_request();
        }

        [Fact]
        public async Task DomainService_deletes_entity_by_id()
        {
            // ARRANGE

            var entity = ArrangeDomainEntity();

            this.ArrangeFindEntityById(entity);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            this.DomainEntityRepositoryMock
                .Setup(r => r.Delete(entity));

            this.DomainModelMock
                .Setup(m => m.SaveChanges())
                .ReturnsAsync(1);

            // ACT

            await base.DomainContract_deletes_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task DomainService_deleting_entity_by_id_returns_false_on_missing_entity()
        {
            // ARRANGE

            this.DomainEntityRepositoryMock
                .Setup(r => r.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((DomainEntity)null);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.DomainContract_deleting_entity_by_id_returns_false_on_missing_entity();
        }

        #endregion Domain Command Path

        #region Domain Query Path

        [Fact]
        public async Task DomainService_reads_entity_by_id()
        {
            // ARRANGE

            var entity = ArrangeDomainEntity();

            this.ArrangeFindEntityById(entity);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.DomainContract_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task DomainService_reading_entity_by_id_fails_on_unknown_id()
        {
            // ARRANGE

            this.DomainEntityRepositoryMock
                .Setup(r => r.FindById(It.IsAny<Guid>()))
                .ReturnsAsync((DomainEntity)null);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.DomainContract_reading_entity_by_id_fails_on_unknown_id();
        }

        [Fact]
        public async Task DomainService_reads_entities()
        {
            // ARRANGE

            var entities = new List<DomainEntity> { ArrangeDomainEntity() };

            this.DomainEntityRepositoryMock
                .Setup(r => r.Query())
                .Returns(entities.ToAsyncEnumerable());

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.DomainContract_reads_entities(entities.Single().Id);
        }

        #endregion Domain Query Path

        #region Domain Events

        [Fact]
        public async Task DomainService_notifies_on_create()
        {
            // ARRANGE

            DomainModelMock
                .Setup(m => m.Entities)
                .Returns(DomainEntityRepositoryMock.Object);

            DomainEntityRepositoryMock
                .Setup(r => r.Add(It.IsAny<DomainEntity>()))
                .Returns(Task.CompletedTask);

            DomainModelMock
                .Setup(m => m.SaveChanges())
                .ReturnsAsync(1);

            // ACT

            await base.DomainContract_notifies_on_create();
        }

        [Fact]
        public async Task DomainService_notifies_on_delete()
        {
            // ARRANGE

            var entity = ArrangeDomainEntity();

            DomainModelMock
                .Setup(m => m.Entities)
                .Returns(DomainEntityRepositoryMock.Object);

            this.ArrangeFindEntityById(entity);

            DomainEntityRepositoryMock
                .Setup(r => r.Delete(entity));

            DomainModelMock
                .Setup(m => m.SaveChanges())
                .ReturnsAsync(1);

            // ACT

            await base.DomainContract_notifies_on_delete(entity.Id);
        }

        #endregion Domain Events

        #region Arrangements

        private void ArrangeFindEntityById(DomainEntity entity)
        {
            this.DomainEntityRepositoryMock
                .Setup(r => r.FindById(entity.Id))
                .ReturnsAsync(entity);
        }

        private static DomainEntity ArrangeDomainEntity()
        {
            return new DomainEntity
            {
                Id = Guid.NewGuid(),
                Text = "test"
            };
        }

        #endregion Arrangements
    }
}