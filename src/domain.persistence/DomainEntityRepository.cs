using domain.model;

namespace domain.persistence
{
    public sealed class DomainEntityRepository : IDomainEntityRepository
    {
        private readonly DomainModel model;

        public DomainEntityRepository(DomainModel model)
        {
            this.model = model;
        }

        public void Add(DomainEntity domainEntity)
        {
            this.model.Added(domainEntity);
        }
    }
}