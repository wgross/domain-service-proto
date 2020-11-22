using System;

namespace Domain.Contract
{
    public enum DomainEntityEventTypes
    {
        Added,
        Modified,
        Deleted
    }

    public sealed class DomainEntityEvent
    {
        public Guid Id { get; set; }

        public DomainEntityEventTypes EventType { get; set; }
    }
}