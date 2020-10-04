using System.Threading.Tasks;

namespace domain.contract
{
    public interface IDomainService
    {
        Task<DoSomethingResult> DoSomething(DoSomethingRequest rq);
    }
}