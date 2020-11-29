using Domain.Contract;
using Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Service
{
    public static class DomainMapper
    {
        public static DomainEntity MapFromRequest(this CreateDomainEntityRequest rq) => new DomainEntity
        {
            Text = rq.Text
        };

        public static DomainEntity MapToEntity(this UpdateDomainEntityRequest rq, DomainEntity e)
        {
            e.Text = rq.Text;
            return e;
        }

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