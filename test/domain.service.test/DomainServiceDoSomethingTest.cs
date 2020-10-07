using domain.contract.test;
using domain.model;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace domain.service.test
{
    public class DomainServiceDoSomethingTest : DomainServiceDoSomethingTestBase, IDisposable
    {
        private MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        private Mock<IDomainModel> DomainModelMock { get; }

        private Mock<IDomainEntityRepository> DomainEntityRepositoryMock { get; }

        public DomainServiceDoSomethingTest()
        {
            this.DomainModelMock = this.Mocks.Create<IDomainModel>();
            this.DomainEntityRepositoryMock = this.Mocks.Create<IDomainEntityRepository>();
            this.Contract = new DomainService(this.DomainModelMock.Object);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly

        public void Dispose() => this.Mocks.VerifyAll();

#pragma warning restore CA1063 // Implement IDisposable Correctly

        [Fact]
        public Task DomainService_does_something() => base.DomainService_does_something_Act();

        [Fact]
        public Task DomainService_doing_someting_fails_on_missing_body() => base.DomainService_doing_someting_fails_on_missing_body_Act();

        [Fact]
        public Task DomainService_doing_something_fails_on_bad_input() => base.DomainService_doing_something_fails_on_bad_input_Act();

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

            await base.DomainService_creates_entity_Act();
        }

        [Fact]
        public async Task DomainService_reads_entity_by_id()
        {
            // ARRANGE

            var entity = new DomainEntity
            {
                Id = Guid.NewGuid(),
                Text = "test"
            };

            this.DomainEntityRepositoryMock
                .Setup(r => r.FindById(entity.Id))
                .ReturnsAsync(entity);

            this.DomainModelMock
                .Setup(m => m.Entities)
                .Returns(this.DomainEntityRepositoryMock.Object);

            // ACT

            await base.DomainService_reads_entity_by_id_Act(entity.Id);
        }
    }
}