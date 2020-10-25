using System;
using System.Threading.Tasks;

namespace domain.model
{
    public interface IDomainModel : IDisposable
    {
        IDomainEntityRepository Entities { get; }

        Task<int> SaveChanges();
    }
}