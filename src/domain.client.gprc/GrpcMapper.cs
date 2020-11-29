using Domain.Contract;
using Domain.Contract.Proto;
using System;
using System.Linq;

namespace Domain.Client.Gprc
{
    public static class GrpcMapper
    {
        public static GrpcCreateDomainEntityRequest ToGrpcMessage(this CreateDomainEntityRequest rq)
        {
            if (rq is null)
                return null;

            return new GrpcCreateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static GrpcUpdateDomainEntityRequest ToGrpcMessage(this UpdateDomainEntityRequest rq, Guid id)
        {
            if (rq is null)
                return null;

            return new GrpcUpdateDomainEntityRequest
            {
                Id = id.ToString(),
                Text = rq.Text
            };
        }

        public static DomainEntityResult FromGrpcMessage(this GrpcDomainEntityResponse result)
        {
            return new DomainEntityResult
            {
                Id = Guid.Parse(result.Id),
                Text = result.Text
            };
        }

        public static DomainEntityCollectionResult FromGrpcMessage(this GrpcDomainEntityCollectionResponse result)
        {
            return new DomainEntityCollectionResult
            {
                Entities = result.Entities.Select(e => e.FromGrpcMessage()).ToArray()
            };
        }

        public static bool FromGrpcMessage(this GrpcDeleteDomainEntityResponse result)
        {
            return result.Success;
        }

        public static DomainEntityEvent FromGrpcMessage(this GrpcDomainEntityEvent result)
        {
            return new DomainEntityEvent
            {
                Id = Guid.Parse(result.Id),
                EventType = (DomainEntityEventTypes)result.EventType
            };
        }
    }
}