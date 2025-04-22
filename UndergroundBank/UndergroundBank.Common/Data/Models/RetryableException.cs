namespace UndergroundBank.Common.Data.Models
{
    public class RetryableException : Exception
    {
        public RetryableException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
