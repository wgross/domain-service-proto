using domain.contract;
using domain.model;

namespace domain.service
{
    public static class DomainMapper
    {
        public static DomainEntity MapToDomain(this CreateDomainEntityRequest rq) => new DomainEntity
        {
            Text = rq.Text
        };

        public static CreateDomainEntityResult MapToResponse(this DomainEntity entity) => new CreateDomainEntityResult
        {
            Id = entity.Id,
            Text = entity.Text
        };
    }
}