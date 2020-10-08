using System;
using System.Threading.Tasks;

namespace domain.contract
{
    public interface IDomainService
    {
        Task<DoSomethingResult> DoSomething(DoSomethingRequest rq);

        Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity);

        Task<DomainEntityResult> GetEntity(Guid id);

        Task DeleteEntity(Guid entityId);

        Task<DomainEntityCollectionResult> GetEntities();
    }
}