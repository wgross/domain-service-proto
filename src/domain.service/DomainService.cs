using Domain.Contract;
using Domain.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Domain.Service
{
    /// <inheritdoc/>
    public class DomainService : IDomainService
    {
        private readonly IDomainModel model;
        private readonly ILogger<DomainService> logger;

        public DomainService(IDomainModel model, ILogger<DomainService> logger)
        {
            this.model = model;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            if (createDomainEntity is null)
                throw new ArgumentNullException(nameof(createDomainEntity));

            this.logger.LogDebug("Creating Entity({text})", createDomainEntity.Text);

            var entity = createDomainEntity.MapFromRequest();
            await this.model.Entities.Add(entity);
            await this.model.SaveChanges();

            this.Publish(new DomainEntityEvent
            {
                Id = entity.Id,
                EventType = DomainEntityEventTypes.Added
            });

            Log.DomainEntityCreated(this.logger, createDomainEntity.Text, null);

            return entity.MapToResponse();
        }

        /// <inheritdoc/>
        public async Task<DomainEntityResult> UpdateEntity(Guid id, UpdateDomainEntityRequest updateDomainEntity)
        {
            if (updateDomainEntity is null)
                throw new ArgumentNullException(nameof(updateDomainEntity));

            this.logger.LogDebug("Updating Entity({id})", id);

            var entity = await this.model.Entities.FindById(id);
            if (entity is null)
                throw new DomainEntityMissingException($"{nameof(DomainEntity)}(id={id}) not found");

            updateDomainEntity.MapToEntity(entity);

            await this.model.SaveChanges();

            this.Publish(new DomainEntityEvent
            {
                Id = entity.Id,
                EventType = DomainEntityEventTypes.Modified
            });

            Log.DomainEntityUpdated(this.logger, entity.Id, entity.Text, null);

            return entity.MapToResponse();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEntity(Guid id)
        {
            var entity = await this.model.Entities.FindById(id);
            if (entity is null)
                return false;

            this.logger.LogDebug("Deleting Entity({id})", id);

            this.model.Entities.Delete(entity);

            await this.model.SaveChanges();

            this.Publish(new DomainEntityEvent
            {
                Id = entity.Id,
                EventType = DomainEntityEventTypes.Deleted
            });

            Log.DomainEntityDeleted(this.logger, entity.Id, null);

            return true;
        }

        /// <inheritdoc/>
        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            if (rq is null)
                throw new ArgumentNullException(nameof(rq));

            if (rq.Data is null)
                throw new InvalidOperationException("Data is required");

            return Task.FromResult(new DoSomethingResult());
        }

        /// <inheritdoc/>
        public Task<DomainEntityCollectionResult> GetEntities()
        {
            return this.model.Entities.Query().MapToResponse();
        }

        /// <inheritdoc/>
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

        #region Prepared log events

        private class Log
        {
            public static readonly EventId Created = new EventId(100, nameof(Created));
            public static readonly EventId Updated = new EventId(101, nameof(Updated));
            public static readonly EventId Deleted = new EventId(102, nameof(Deleted));

            public static Action<ILogger, string, Exception> DomainEntityCreated = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information,
                eventId: Created,
                formatString: "Created DomainEntity(text={text})");

            public static Action<ILogger, Guid, string, Exception> DomainEntityUpdated = LoggerMessage.Define<Guid, string>(
                 logLevel: LogLevel.Information,
                 eventId: Updated,
                 formatString: "Updated DomainEntity(id={id},text={text})");

            public static Action<ILogger, Guid, Exception> DomainEntityDeleted = LoggerMessage.Define<Guid>(
                logLevel: LogLevel.Information,
                eventId: Deleted,
                formatString: "Deleted DomainEntity(id={id})");
        }

        #endregion Prepared log events
    }
}