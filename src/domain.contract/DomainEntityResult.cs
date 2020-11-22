using System;

namespace Domain.Contract
{
    public sealed class DomainEntityResult
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}