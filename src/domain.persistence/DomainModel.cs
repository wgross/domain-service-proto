using domain.model;
using domain.persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
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

        public DatabaseFacade Database => this.DbContext.Database;

        public async Task<int> SaveChanges()
        {
            var events = this.DbContext.ChangeTracker
                .Entries<DomainEntity>()
                .Select(e => e.State switch
                {
                    EntityState.Added => new DomainEvent
                    {
                        Event = DomainEventValues.Added,
                        Data = e.Entity
                    },
                    EntityState.Modified => new DomainEvent
                    {
                        Event = DomainEventValues.Modified,
                        Data = e.Entity
                    },
                    EntityState.Deleted => new DomainEvent
                    {
                        Event = DomainEventValues.Deleted,
                        Data = e.Entity
                    }
                })
                .ToList();

            var affected = await this.DbContext.SaveChangesAsync();

            // TODO: don't send the entity. Send the Id to avoid side effects
            events.ForEach(e => domainEvents.OnNext(e));

            return affected;
        }

        public void Dispose() => this.DbContext.Dispose();
    }
}