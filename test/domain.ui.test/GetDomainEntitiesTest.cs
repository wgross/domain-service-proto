using Bunit;
using Domain.Contract;
using Domain.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Domain.UI.Test
{
    public class GetDomainEntitiesTest : IDisposable
    {
        private readonly TestContext testContext;
        private readonly MockRepository mocks;

        public GetDomainEntitiesTest()
        {
            this.testContext = new TestContext();
            this.mocks = new MockRepository(MockBehavior.Strict);
        }

        public void Dispose() => this.mocks.VerifyAll();

        [Fact]
        public void GetDomainEntities_gets_all_entities_on_init()
        {
            // ARRANGE

            var entities = new DomainEntityCollectionResult
            {
                Entities = new[]
                    {
                    new DomainEntityResult
                    {
                        Id = Guid.NewGuid(),
                        Text = "text"
                    }
                }
            };

            var domainClient = this.mocks.Create<IDomainService>();
            domainClient
                .Setup(c => c.GetEntities())
                .ReturnsAsync(entities);

            this.testContext.Services.AddSingleton<IDomainService>(domainClient.Object);

            // ACT

            var result = this.testContext.RenderComponent<GetDomainEntities>();

            // ASSERT

            result.Find("table").MarkupMatches($@"
<table class=""table"">
    <thead>
        <tr>
            <th>Id</th>
            <th>Text</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>{entities.Entities.Single().Id}</td>
            <td>{entities.Entities.Single().Text}</td>
        </tr>
    </tbody>
</table>");
        }
    }
}