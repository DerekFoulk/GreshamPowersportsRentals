using System.Runtime.Serialization;

namespace Data.Specialized.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a table element contains elements that are unexpected.
    /// </summary>
    public class UnexpectedTableLayoutException : Exception
    {
        public UnexpectedTableLayoutException()
        {
        }

        public UnexpectedTableLayoutException(string? message) : base(message)
        {
        }

        public UnexpectedTableLayoutException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
