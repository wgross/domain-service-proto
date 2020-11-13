using domain.contract;
using domain.contract.proto;
using System;

namespace domain.client.gprc
{
    public static class GrpcMapper
    {
        public static GCreateDomainEntityRequest ToGrpcMessage(this CreateDomainEntityRequest rq)
        {
            if (rq is null)
                return null;

            return new GCreateDomainEntityRequest
            {
                Text = rq.Text
            };
        }

        public static GDeleteDomainEntityRequest ToGrpcMessage(this Guid rq)
        {
            return new GDeleteDomainEntityRequest
            {
                Id = rq.ToString()
            };
        }

        public static DomainEntityResult FromGrpcMessage(this GCreateDomainEntityResponse result)
        {
            return new DomainEntityResult
            {
                Id = Guid.Parse(result.Id),
                Text = result.Text
            };
        }

        public static bool FromGrpcMessage(this GDeleteDomainEntityResponse result)
        {
            return result.Success;
        }
    }
}