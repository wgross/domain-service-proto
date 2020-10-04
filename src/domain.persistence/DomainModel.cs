using domain.model;
using System;
using System.Reactive.Subjects;

namespace domain.persistence
{
    public sealed class DomainModel : IDomainModel
    {
        private static readonly Subject<DomainEvent> domainEvents = new Subject<DomainEvent>();

        public IDomainEntityRepository Entities => new DomainEntityRepository(this);

        public IObservable<DomainEvent> DomainEvents => domainEvents;

        internal void Added(DomainEntity entity)
        {
            domainEvents.OnNext(new DomainEvent
            {
                Event = nameof(Added),
                Data = entity
            });
        }
    }
}