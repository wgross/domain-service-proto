using System;
using System.Threading.Tasks;

namespace Domain.Model
{
    public interface IDomainModel : IDisposable
    {
        IDomainEntityRepository Entities { get; }

        Task<int> SaveChanges();
    }
}