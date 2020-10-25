using domain.contract;
using domain.model;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace domain.service
{
    public class DomainService : IDomainService
    {
        private readonly IDomainModel model;

        public DomainService(IDomainModel model)
        {
            this.model = model;
        }

        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            var entity = createDomainEntity.MapToDomain();
            await this.model.Entities.Add(entity);
            await this.model.SaveChanges();

            this.Publish(new DomainEntityEvent
            {
                Id = entity.Id,
                EventType = DomainEntityEventTypes.Added
            });

            return entity.MapToResponse();
        }

        public async Task DeleteEntity(Guid entityId)
        {
            var entity = await this.model.Entities.FindById(entityId);
            this.model.Entities.Delete(entity);
            await this.model.SaveChanges();

            this.Publish(new DomainEntityEvent
            {
                Id = entity.Id,
                EventType = DomainEntityEventTypes.Deleted
            });
        }

        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            if (rq is null)
                throw new ArgumentNullException(nameof(rq));

            if (rq.Data is null)
                throw new InvalidOperationException("Data is required");

            return Task.FromResult(new DoSomethingResult());
        }

        public Task<DomainEntityCollectionResult> GetEntities()
        {
            return this.model.Entities.Query().MapToResponse();
        }

        public async Task<DomainEntityResult> GetEntity(Guid id)
        {
            var entity = await this.model.Entities.FindById(id);
            return entity?.MapToResponse();
        }

        #region Domain Events

        private static readonly object domainEventsSync = new object();

        private static readonly ISubject<DomainEntityEvent> domainEvents = new Subject<DomainEntityEvent>();

        private void Publish(DomainEntityEvent domainEntityEvent)
        {
            lock (domainEventsSync)
                domainEvents.OnNext(domainEntityEvent);
        }

        public Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> events)
        {
            return Task.Run(() =>
            {
                lock (domainEventsSync)
                    return domainEvents.Subscribe(events);
            });
        }

        #endregion Domain Events
    }
}