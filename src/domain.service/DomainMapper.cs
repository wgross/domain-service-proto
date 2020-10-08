using domain.contract;
using domain.model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async static Task<DomainEntityCollectionResult> MapToResponse(this IAsyncEnumerable<DomainEntity> collection)
        {
            return new DomainEntityCollectionResult
            {
                Entities = await collection.Select(e => e.MapToResponse()).ToArrayAsync()
            };
        }
    }
}