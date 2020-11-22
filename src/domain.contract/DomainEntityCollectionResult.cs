using System.Collections.Generic;

namespace Domain.Contract
{
    public class DomainEntityCollectionResult
    {
        public IEnumerable<DomainEntityResult> Entities { get; set; }
    }
}