using System;

namespace domain.contract
{
    public sealed class CreateDomainEntityResult
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}