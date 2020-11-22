namespace Domain.Contract
{
    public class DomainError
    {
        public string ErrorType { get; set; }

        public string Message { get; set; }

        public string ParamName { get; set; }
    }
}