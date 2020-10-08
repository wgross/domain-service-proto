using domain.contract;
using domain.model;
using System;
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

        public async Task<CreateDomainEntityResult> CreateEntity(CreateDomainEntityRequest createDomainEntity)
        {
            var entity = createDomainEntity.MapToDomain();
            await this.model.Entities.Add(entity);
            await this.model.SaveChanges();
            return entity.MapToResponse();
        }

        public async Task DeleteEntity(Guid entityId)
        {
            var entity = await this.model.Entities.FindById(entityId);
            this.model.Entities.Delete(entity);
            await this.model.SaveChanges();
        }

        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            if (rq is null)
                throw new ArgumentNullException(nameof(rq));

            if (rq.Data is null)
                throw new InvalidOperationException("Data is required");

            return Task.FromResult(new DoSomethingResult());
        }

        public async Task<CreateDomainEntityResult> GetEntity(Guid id)
        {
            var entity = await this.model.Entities.FindById(id);
            return entity.MapToResponse();
        }
    }
}