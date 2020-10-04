using System;

namespace domain.model
{
    public interface IDomainModel
    {
        IDomainEntityRepository Entities { get; }

        IObservable<DomainEvent> DomainEvents { get; }
    }
}