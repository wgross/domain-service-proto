using Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Persistence
{
    public sealed class DomainEntityRepository : IDomainEntityRepository
    {
        private readonly DomainModel model;

        public DomainEntityRepository(DomainModel model)
        {
            this.model = model;
        }

        public async Task Add(DomainEntity domainEntity) => await this.model.DbContext.DomainEntities.AddAsync(domainEntity);

        public void Delete(DomainEntity actEntity) => this.model.DbContext.DomainEntities.Remove(actEntity);

        public ValueTask<DomainEntity> FindById(Guid id) => this.model.DbContext.DomainEntities.FindAsync(id);

        public IAsyncEnumerable<DomainEntity> Query() => this.model.DbContext.DomainEntities.AsAsyncEnumerable();
    }
}