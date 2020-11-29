using System;
using System.Threading.Tasks;

namespace Domain.Contract
{
    public interface IDomainService
    {
        Task<DoSomethingResult> DoSomething(DoSomethingRequest rq);

        Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity);

        Task<DomainEntityResult> UpdateEntity(Guid id, UpdateDomainEntityRequest updateDomainEntity);

        Task<DomainEntityResult> GetEntity(Guid id);

        Task<bool> DeleteEntity(Guid entityId);

        Task<DomainEntityCollectionResult> GetEntities();

        Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> events);
    }
}