namespace Data.Specialized.Exceptions
{
    public class GeometryNotFoundException : Exception
    {
        public GeometryNotFoundException()
        {
        }

        public GeometryNotFoundException(string? message) : base(message)
        {
        }

        public GeometryNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
