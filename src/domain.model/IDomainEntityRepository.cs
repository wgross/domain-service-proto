using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace domain.model
{
    public interface IDomainEntityRepository
    {
        Task Add(DomainEntity domainEntity);
    }
}
