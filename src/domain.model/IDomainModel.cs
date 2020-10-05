using System;

namespace domain.model
{
    public interface IDomainModel : IDisposable
    {
        IDomainEntityRepository Entities { get; }

        IObservable<DomainEvent> DomainEvents { get; }
    }
}