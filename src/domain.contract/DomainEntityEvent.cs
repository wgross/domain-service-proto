using System;

namespace domain.contract
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