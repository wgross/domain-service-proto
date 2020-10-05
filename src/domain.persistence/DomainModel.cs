using domain.model;
using domain.persistence.EF;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace domain.persistence
{
    public sealed class DomainModel : IDomainModel
    {
        private static readonly Subject<DomainEvent> domainEvents = new Subject<DomainEvent>();

        public DomainModel(DomainDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        internal DomainDbContext DbContext { get; }

        public IDomainEntityRepository Entities => new DomainEntityRepository(this);

        public IObservable<DomainEvent> DomainEvents => domainEvents;

        public Task<int> SaveChanges() => this.DbContext.SaveChangesAsync();

        internal void Added(DomainEntity entity)
        {
            domainEvents.OnNext(new DomainEvent
            {
                Event = nameof(Added),
                Data = entity
            });
        }

        public void Dispose() => this.DbContext.Dispose();
    }
}