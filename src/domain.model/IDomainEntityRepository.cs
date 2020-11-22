using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IDomainEntityRepository
    {
        Task Add(DomainEntity domainEntity);

        IAsyncEnumerable<DomainEntity> Query();

        ValueTask<DomainEntity> FindById(Guid id);

        void Delete(DomainEntity actEntity);
    }
}