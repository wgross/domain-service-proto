using domain.model;
using System.Threading.Tasks;

namespace domain.persistence
{
    public sealed class DomainEntityRepository : IDomainEntityRepository
    {
        private readonly DomainModel model;

        public DomainEntityRepository(DomainModel model)
        {
            this.model = model;
        }

        public async Task Add(DomainEntity domainEntity)
        {
            await this.model.DbContext.DomainEntities.AddAsync(domainEntity);
            this.model.Added(domainEntity);
        }
    }
}