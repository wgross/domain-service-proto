using Domain.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Domain.Host.Controllers.Test
{
    /// <summary>
    /// At the controller level a HTTP-conformant behavior is enforced.
    /// Mainly this includes the proper status codes for responses to allow client which arn't
    /// using the client library to interact in a predictable way with the web service.
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
        public async Task DomainController_creating_entity_returns_path()
        {
            // ARRANGE

            var entityId = Guid.NewGuid();
            var createEntityRequest = new CreateDomainEntityRequest
            {
                Text = "text"
            };

            this.DomainServiceMock
                .Setup(m => m.CreateEntity(createEntityRequest))
                .ReturnsAsync(new DomainEntityResult
                {
                    Id = entityId,
                    Text = createEntityRequest.Text
                });

            // ACT

            var result = await this.controller.CreateEntity(createEntityRequest) as CreatedAtActionResult;

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(createEntityRequest.Text, ((DomainEntityResult)result.Value).Text);
            Assert.Equal(entityId, ((DomainEntityResult)result.Value).Id);
            Assert.Equal(nameof(DomainController.GetEntity), result.ActionName);
            Assert.Equal(entityId, result.RouteValues["id"]);
        }

        [Fact]
        public async Task DomainController_creating_entity_fails_on_null_data_returns_BadRequest()
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

            Assert.Equal(exception.Message, resultValue.Message);
            Assert.Equal(exception.GetType().Name, resultValue.ErrorType);
            Assert.Equal(exception.ParamName, resultValue.ParamName);
        }

        [Fact]
        public async Task DomainController_updates_entity()
        {
            // ARRANGE

            var entityId = Guid.NewGuid();
            var updateEntityRequest = new UpdateDomainEntityRequest { Text = "text-changed" };

            this.DomainServiceMock
                .Setup(m => m.UpdateEntity(entityId, updateEntityRequest))
                .ReturnsAsync(new DomainEntityResult
                {
                    Id = entityId,
                    Text = updateEntityRequest.Text
                });

            // ACT

            var result = await this.controller.UpdateEntity(entityId, updateEntityRequest) as OkObjectResult;

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal(updateEntityRequest.Text, ((DomainEntityResult)result.Value).Text);
            Assert.Equal(entityId, ((DomainEntityResult)result.Value).Id);
        }

        [Fact]
        public async Task DomainController_updating_entity_fails_on_null_data_returns_BadRequest()
        {
            // ARRANGE

            var exception = new ArgumentNullException("updateDomainEntity");
            var entityId = Guid.NewGuid();
            this.DomainServiceMock
                .Setup(m => m.UpdateEntity(entityId, null))
                .ThrowsAsync(exception);

            // ACT

            var result = await this.controller.UpdateEntity(entityId, null) as BadRequestObjectResult;

            // ASSERT

            var resultValue = result.Value as DomainError;

            Assert.Equal(exception.Message, resultValue.Message);
            Assert.Equal(exception.GetType().Name, resultValue.ErrorType);
            Assert.Equal(exception.ParamName, resultValue.ParamName);
        }

        [Fact]
        public async Task DomainController_updating_entity_returns_NotFound_on_missing_entity()
        {
            // ARRANGE

            var exception = new DomainEntityMissingException("updateDomainEntity");
            var entityId = Guid.NewGuid();
            this.DomainServiceMock
                .Setup(m => m.UpdateEntity(entityId, null))
                .ThrowsAsync(exception);

            // ACT

            var result = await this.controller.UpdateEntity(entityId, null) as NotFoundObjectResult;

            // ASSERT

            var resultValue = result.Value as DomainError;

            Assert.Equal(exception.Message, resultValue.Message);
            Assert.Equal(exception.GetType().Name, resultValue.ErrorType);
            Assert.Null(resultValue.ParamName);
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
        public async Task DomainController_deleting_entity_by_id_returns_NotFound_on_missing_entity()
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