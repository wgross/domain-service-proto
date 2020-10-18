using domain.host.controllers;
using domain.model;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.test
{
    public class DomainEventControllerTest
    {
        private readonly DomainEventController controller;

        private MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        private Mock<IDomainModel> DomainModelMock { get; }

        public DomainEventControllerTest()
        {
            this.DomainModelMock = this.Mocks.Create<IDomainModel>();

            this.controller = new DomainEventController(this.DomainModelMock.Object);
        }

        [Fact]
        public async Task DomainEventControler_sends_create_event()
        {
            // ARRANGE

            var cancellationTokenSource = new CancellationTokenSource();

            // ACT

            await this.controller.Get(cancellationTokenSource.Token);
        }
    }
}