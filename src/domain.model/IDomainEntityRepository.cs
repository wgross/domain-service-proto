using System;
using System.Collections.Generic;
using System.Text;

namespace domain.model
{
    public interface IDomainEntityRepository
    {
        void Add(DomainEntity domainEntity);
    }
}
