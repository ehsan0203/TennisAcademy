using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class BaseExceptions : Exception
    {
        public int StatusCode { get; set; }

        public BaseExceptions()
        {

        }

        public BaseExceptions(string message)
            : base(message)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
