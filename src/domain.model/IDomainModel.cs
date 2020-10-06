using System;
using System.Threading.Tasks;

namespace domain.model
{
    public interface IDomainModel : IDisposable
    {
        IDomainEntityRepository Entities { get; }

        IObservable<DomainEvent> DomainEvents { get; }

        Task<int> SaveChanges();
    }
}