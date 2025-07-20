using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class NotSetConfigException : BaseExceptions
    {
        public NotSetConfigException()
        {

        }

        public NotSetConfigException(string configkey)
            : base("NotSetConfigException => " + configkey)
        {
            StatusCode = (int)HttpStatusCode.ServiceUnavailable;
        }
    }
}
