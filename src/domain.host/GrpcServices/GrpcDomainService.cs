using Domain.Contract;
using Domain.Contract.Proto;
using Grpc.Core;
using System.Threading.Tasks;
using static Domain.Contract.Proto.GDomainService;

namespace Domain.Host.GrpcServices
{
    public class GrpcDomainService : GDomainServiceBase
    {
        private readonly IDomainService domainService;

        public GrpcDomainService(IDomainService domainService)
        {
            this.domainService = domainService;
        }

        public override async Task<GCreateDomainEntityResponse> Create(GCreateDomainEntityRequest request, ServerCallContext context)
        {
            return (await this.domainService.CreateEntity(request.MapFromMessage())).MapToMessage();
        }

        public override async Task<GDeleteDomainEntityResponse> Delete(GDeleteDomainEntityRequest request, ServerCallContext context)
        {
            return (await this.domainService.DeleteEntity(request.MapFromMessage())).MapToMessage();
        }
    }
}