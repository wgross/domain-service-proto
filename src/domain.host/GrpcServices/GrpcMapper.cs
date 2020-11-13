using domain.contract;
using domain.contract.proto;
using System;

namespace domain.host.GrpcServices
{
    public static class GrpcMapper
    {
        public static CreateDomainEntityRequest MapFromMessage(this contract.proto.GCreateDomainEntityRequest rq)
        {
            return new CreateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static Guid MapFromMessage(this contract.proto.GDeleteDomainEntityRequest rq)
        {
            return Guid.Parse(rq.Id);
        }

        public static contract.proto.GCreateDomainEntityResponse MapToMessage(this DomainEntityResult response)
        {
            return new GCreateDomainEntityResponse
            {
                Id = response.Id.ToString(),
                Text = response.Text
            };
        }

        public static GDeleteDomainEntityResponse MapToMessage(this bool response)
        {
            return new GDeleteDomainEntityResponse
            {
                Success = response
            };
        }
    }
}