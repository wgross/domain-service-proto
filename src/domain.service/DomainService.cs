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

            return Task.FromResult(new DoSomethingResult());
        }
    }
}