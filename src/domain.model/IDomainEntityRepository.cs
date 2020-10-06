using System;
using System.Linq;
using System.Threading.Tasks;

namespace domain.model
{
    public interface IDomainEntityRepository
    {
        Task Add(DomainEntity domainEntity);

        IQueryable<DomainEntity> Query();

        ValueTask<DomainEntity> FindById(Guid id);

        void Delete(DomainEntity actEntity);
    }
}