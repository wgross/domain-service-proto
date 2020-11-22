namespace Domain.Model
{
    public enum DomainEventValues
    {
        Added,
        Modified,
        Deleted
    }

    public class DomainEvent
    {
        public DomainEventValues Event { get; set; }

        public DomainEntity Data { get; set; }
    }
}