namespace BuildingBlocks.Exceptions;

public class ForbiddenAccessException : Exception
{
    public string Message { get; set; }
    public int ErrorCode { get; set; }
    public ForbiddenAccessException(string message, int errorCode =0) : base(message)
    {
        Message = message;
        ErrorCode = errorCode;
    }


}
