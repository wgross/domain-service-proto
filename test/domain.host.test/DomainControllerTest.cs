using domain.contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.controllers.test
{
    public class DomainControllerTest
    {
        private readonly DomainController controller;
        private MockRepository Mocks { get; } = new MockRepository(MockBehavior.Strict);

        private Mock<IDomainService> DomainServiceMock { get; }

        public DomainControllerTest()
        {
            this.DomainServiceMock = this.Mocks.Create<IDomainService>();

            this.controller = new DomainController(this.DomainServiceMock.Object);
        }

        [Fact]
        public async Task DomainController_create_entity_returns_path()
        {
            // ARRANGE

            var entityId = Guid.NewGuid();
            this.DomainServiceMock
                .Setup(m => m.CreateEntity(It.IsAny<CreateDomainEntityRequest>()))
                .ReturnsAsync(new DomainEntityResult
                {
                    Id = entityId
                });

            // ACT

            var result = await this.controller.CreateEntity(new CreateDomainEntityRequest()) as CreatedAtActionResult;

            // ASSERT

            Assert.NotNull(result);
            Assert.IsType<DomainEntityResult>(result.Value);
            Assert.Equal(nameof(DomainController.GetEntity), result.ActionName);
            Assert.Equal(entityId, result.RouteValues["id"]);
        }

        [Fact]
        public async Task DomainController_reading_by_id_returns_NotFound_on_unkowen_id()
        {
            // ARRANGE

            this.DomainServiceMock
                .Setup(m => m.GetEntity(It.IsAny<Guid>()))
                .ReturnsAsync((DomainEntityResult)null);

            // ACT

            var result = await this.controller.GetEntity(Guid.NewGuid()) as NotFoundResult;

            // ASSERT

            Assert.NotNull(result);
        }
    }
}