namespace Data.Specialized.Exceptions
{
    public class GeometryTableRowsNotFoundException : Exception
    {
        public GeometryTableRowsNotFoundException()
        {
        }

        public GeometryTableRowsNotFoundException(string? message) : base(message)
        {
        }

        public GeometryTableRowsNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
