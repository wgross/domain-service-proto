using domain.contract;
using System.Threading.Tasks;

namespace domain.service
{
    public class DomainService : IDomainService
    {
        public Task<DoSomethingResult> DoSomething(DoSomethingRequest rq)
        {
            return Task.FromResult(new DoSomethingResult());
        }
    }
}