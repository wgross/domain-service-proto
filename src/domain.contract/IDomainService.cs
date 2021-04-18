using System;
using System.Threading.Tasks;

namespace Domain.Contract
{
    /// <summary>
    /// Abstraction of the Domain Behavior.
    /// </summary>
    public interface IDomainService
    {
        Task<DoSomethingResult> DoSomething(DoSomethingRequest rq);

        /// <summary>
        /// Creates a domain entity"/>
        /// </summary>
        Task<DomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity);

        /// <summary>
        /// Modifies a domain entities state
        /// </summary>
        Task<DomainEntityResult> UpdateEntity(Guid id, UpdateDomainEntityRequest updateDomainEntity);

        /// <summary>
        /// Reads an entity by ID
        /// </summary>
        Task<DomainEntityResult> GetEntity(Guid id);

        /// <summary>
        /// Deletes an entity specified by id
        /// </summary>
        Task<bool> DeleteEntity(Guid id);

        /// <summary>
        /// Retrieves all domain entities
        /// </summary>
        Task<DomainEntityCollectionResult> GetEntities();

        /// <summary>
        /// Subscribes to entity change events
        /// </summary>
        Task<IDisposable> Subscribe(IObserver<DomainEntityEvent> events);
    }
}