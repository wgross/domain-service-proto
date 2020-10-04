using domain.contract.test;

namespace domain.service.test
{
    public class DomainServiceDoSomethingTest : DomainServiceDoSomethingTestBase
    {
        private readonly DomainService domainService;

        public DomainServiceDoSomethingTest()
        {
            this.Contract = new DomainService();
        }
    }
}