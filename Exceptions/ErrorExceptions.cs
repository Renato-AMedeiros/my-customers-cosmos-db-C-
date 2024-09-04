namespace my_customers_cosmos_db_C_.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }

    }
}
