using System;

namespace domain.contract
{
    public sealed class DomainEntityResult
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}