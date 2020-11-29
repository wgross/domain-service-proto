using Domain.Contract;
using Domain.Contract.Proto;
using System;
using System.Linq;

namespace Domain.Host.GrpcServices
{
    public static class GrpcMapper
    {
        public static CreateDomainEntityRequest FromGrpcMessage(this GrpcCreateDomainEntityRequest rq)
        {
            return new CreateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static UpdateDomainEntityRequest FromGrpcMessage(this GrpcUpdateDomainEntityRequest rq)
        {
            return new UpdateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static Guid FromGrpcMessage(this GrpcGetDomainEntityByIdRequest rq) => Guid.Parse(rq.Id);

        public static Guid FromGrpcMessage(this GrpcDeleteDomainEntityRequest rq) => Guid.Parse(rq.Id);

        public static Contract.Proto.GrpcDomainEntityResponse ToGrpcMessage(this DomainEntityResult response)
        {
            return new GrpcDomainEntityResponse
            {
                Id = response.Id.ToString(),
                Text = response.Text
            };
        }

        public static GrpcDeleteDomainEntityResponse ToGrpcMessage(this bool response)
        {
            return new GrpcDeleteDomainEntityResponse
            {
                Success = response
            };
        }

        public static GrpcDomainEntityCollectionResponse ToGrpcMessage(this DomainEntityCollectionResult response)
        {
            var tmp = new GrpcDomainEntityCollectionResponse();
            tmp.Entities.AddRange(response.Entities.Select(e => e.ToGrpcMessage()));
            return tmp;
        }

        public static GrpcDomainEntityEvent ToGrpcMessage(this DomainEntityEvent response)
        {
            return new GrpcDomainEntityEvent
            {
                Id = response.Id.ToString(),
                EventType = (GrpcDomainEntityEvent.Types.EventType)response.EventType
            };
        }
    }
}