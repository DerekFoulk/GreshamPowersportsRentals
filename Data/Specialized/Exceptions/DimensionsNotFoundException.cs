namespace Data.Specialized.Exceptions
{
    public class DimensionsNotFoundException : Exception
    {
        public DimensionsNotFoundException()
        {
        }

        public DimensionsNotFoundException(string? message) : base(message)
        {
        }

        public DimensionsNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
