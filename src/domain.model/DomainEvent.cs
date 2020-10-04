namespace domain.model
{
    public class DomainEvent
    {
        public string Event { get; set; }
        public DomainEntity Data { get; set; }
    }
}