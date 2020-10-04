using domain.contract;
using System;
using System.Threading.Tasks;

namespace domain.service
{
    public class DomainService : IDomainService
    {
        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            if (rq is null)
                throw new ArgumentNullException(nameof(rq));

            if (rq.Data is null)
                throw new InvalidOperationException("Data is required");

            return Task.FromResult(new DoSomethingResult());
        }
    }
}