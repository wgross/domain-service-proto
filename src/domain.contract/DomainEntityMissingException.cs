using System;

namespace Domain.Contract
{
    public sealed class DomainEntityMissingException : Exception
    {
        public DomainEntityMissingException(string message) : base(message)
        {
        }

        public DomainEntityMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}