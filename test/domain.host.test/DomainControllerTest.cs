using domain.contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace domain.host.controllers.test
{
    /// <summary>
    /// At the controller level a HTTP-conformant bahavior is enforced.
    /// Mainly this includes the proper status codes for resposes to allow client not using the client library
    /// to interact ina predictable way.
    /// </summary>
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
        public async Task DomainController_creating_entity_failing_on_null_data_returns_BadRequest()
        {
            // ARRANGE

            var exception = new ArgumentNullException("createDomainEntity");
            var entityId = Guid.NewGuid();
            this.DomainServiceMock
                .Setup(m => m.CreateEntity(null))
                .ThrowsAsync(exception);

            // ACT

            var result = await this.controller.CreateEntity(null) as BadRequestObjectResult;

            // ASSERT

            var resultValue = result.Value as DomainError;

            // ASSERT

            Assert.Equal(exception.Message, resultValue.Message);
            Assert.Equal(exception.GetType().Name, resultValue.ErrorType);
            Assert.Equal(exception.ParamName, resultValue.ParamName);
        }

        [Fact]
        public async Task DomainController_reading_by_id_returns_NotFound_on_unknown_id()
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

        [Fact]
        public async Task DomainController_deleting_entity_by_id_returns_NoContent()
        {
            // ARRANGE

            this.DomainServiceMock
                .Setup(m => m.DeleteEntity(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // ACT

            var result = await this.controller.DeleteEntity(Guid.NewGuid()) as NoContentResult;

            // ASSERT

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DomainController_deleting_entity_by_id_returns_NotFound_on_missng_entity()
        {
            // ARRANGE

            this.DomainServiceMock
                .Setup(m => m.DeleteEntity(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // ACT

            var result = await this.controller.DeleteEntity(Guid.NewGuid()) as NotFoundResult;

            // ASSERT

            Assert.NotNull(result);
        }
    }
}