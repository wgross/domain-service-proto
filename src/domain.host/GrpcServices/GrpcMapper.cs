using Domain.Contract;
using Domain.Contract.Proto;
using System;

namespace Domain.Host.GrpcServices
{
    public static class GrpcMapper
    {
        public static CreateDomainEntityRequest MapFromMessage(this Contract.Proto.GCreateDomainEntityRequest rq)
        {
            return new CreateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static Guid MapFromMessage(this Contract.Proto.GDeleteDomainEntityRequest rq)
        {
            return Guid.Parse(rq.Id);
        }

        public static Contract.Proto.GCreateDomainEntityResponse MapToMessage(this DomainEntityResult response)
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