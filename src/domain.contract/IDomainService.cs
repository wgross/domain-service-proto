using System;
using System.Threading.Tasks;

namespace domain.contract
{
    public interface IDomainService
    {
        Task<DoSomethingResult> DoSomething(DoSomethingRequest rq);

        Task<CreateDomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity);
        
        Task<CreateDomainEntityResult> GetEntity(Guid id);
    }
}