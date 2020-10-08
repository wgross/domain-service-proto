using domain.contract;
using domain.model;
using System.Linq;

namespace domain.service
{
    public static class DomainMapper
    {
        public static DomainEntity MapToDomain(this CreateDomainEntityRequest rq) => new DomainEntity
        {
            Text = rq.Text
        };

        public static DomainEntityResult MapToResponse(this DomainEntity entity) => new DomainEntityResult
        {
            Id = entity.Id,
            Text = entity.Text
        };

        public static DomainEntityCollectionResult MapToResponse(this IQueryable<DomainEntity> collection)
        {
            return new DomainEntityCollectionResult
            {
                Entities = collection.Select(e => e.MapToResponse()).ToArray()
            };
        }
    }
}