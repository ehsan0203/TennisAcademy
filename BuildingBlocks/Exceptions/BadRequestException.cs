namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : Exception
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public BadRequestException(string message, Exception? exception = null) : base(message, exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}
