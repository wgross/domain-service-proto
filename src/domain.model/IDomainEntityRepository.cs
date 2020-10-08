using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace domain.model
{
    public interface IDomainEntityRepository
    {
        Task Add(DomainEntity domainEntity);

        IAsyncEnumerable<DomainEntity> Query();

        ValueTask<DomainEntity> FindById(Guid id);

        void Delete(DomainEntity actEntity);
    }
}