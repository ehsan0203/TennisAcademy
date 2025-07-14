namespace BuildingBlocks.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }
        public string Message { get; private set; }
        public string? Details { get; private set; }
        public int? ErrorCode { get; set; }
        public string? TraceId { get; set; }
        public ApiException(int statusCode, string message, int? errorCode = null, string? details = null, string? traceId = null, Exception? innerException = null) : base(message, innerException)
        {
            Message = message;
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Details = details;
            TraceId = traceId;
        }
    }
}
