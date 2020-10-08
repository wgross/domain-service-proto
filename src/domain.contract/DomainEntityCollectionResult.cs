using System.Collections.Generic;

namespace domain.contract
{
    public class DomainEntityCollectionResult
    {
        public IEnumerable<DomainEntityResult> Entities { get; set; }
    }
}