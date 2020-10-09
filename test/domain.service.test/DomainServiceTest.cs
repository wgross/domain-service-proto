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
    public class DomainServiceTest : DomainServiceContractTestBase, IDisposable
    {
        private MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        private Mock<IDomainModel> DomainModelMock { get; }

        private Mock<IDomainEntityRepository> DomainEntityRepositoryMock { get; }

        public DomainServiceTest()
        {
            this.DomainModelMock = this.Mocks.Create<IDomainModel>();
            this.DomainEntityRepositoryMock = this.Mocks.Create<IDomainEntityRepository>();
            this.Contract = new DomainService(this.DomainModelMock.Object);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly

        public void Dispose() => this.Mocks.VerifyAll();

#pragma warning restore CA1063 // Implement IDisposable Correctly

        [Fact]
        public Task DomainService_does_something() => base.ACT_DomainService_does_something();

        [Fact]
        public Task DomainService_doing_someting_fails_on_missing_body() => base.ACT_DomainService_doing_someting_fails_on_missing_body();

        [Fact]
        public Task DomainService_doing_something_fails_on_bad_input() => base.ACT_DomainService_doing_something_fails_on_bad_input();

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

            await base.ACT_DomainService_creates_entity();
        }

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

            await base.ACT_DomainService_reads_entity_by_id(entity.Id);
        }

        [Fact]
        public async Task DomainService_reads_entities()
        {
            var entities = new List<DomainEntity> { ArrangeDomainEntity() };

            this.DomainEntityRepositoryMock
                .Setup(r => r.Query())
                .Returns(entities.ToAsyncEnumerable());

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.ACT_DomainService_reads_entities(entities.Single().Id);
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

            await base.ACT_DomainService_deletes_entity_by_id(entity.Id);
        }

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