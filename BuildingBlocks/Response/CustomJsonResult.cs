using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BuildingBlocks.Response
{
    public class CustomJsonResult : IActionResult
    {
        [JsonProperty("data")]
        public object Data { get; private set; }
        public int? PageSize { get; set; } = null;
        public int? PageIndex { get; set; } = null;
        public int? Length { get; set; } = null;
        public bool Status { get; private set; }
        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; private set; }
        public List<string>? ErrorList { get; set; } = null;
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; } // Make the set accessor public
        [JsonIgnore]
        private int statusCode { get; set; }

        [JsonConstructor]
        public CustomJsonResult()
        {

        }

        public CustomJsonResult(object data, int status = StatusCodes.Status200OK, string? errorMessage = null, List<string>? errorList = null)
        {
            Data = data;
            Status = status == StatusCodes.Status200OK ? true : false;
            ErrorMessage = errorMessage;
            ErrorList = errorList;
            statusCode = status;
            StatusCode = status;
        }
        public CustomJsonResult(object data, int pageSize, int pageIndex, int length, int status = StatusCodes.Status200OK, string errorMessage = null)
        {
            Length = length;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data;
            Status = status == StatusCodes.Status200OK ? true : false; ;
            ErrorMessage = errorMessage;
            statusCode = status;
            StatusCode = status;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = new
            {
                status = Status,
                data = Data,
                errorMessage = ErrorMessage,
                errors = ErrorList,
                length = Length,
                pageIndex = PageIndex,
                pageSize = PageSize
            };
            context.HttpContext.Response.StatusCode = statusCode;
            var result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            await result.ExecuteResultAsync(context);
        }
    }
    public class CustomJsonResult<T> : IActionResult
    {
        public T Data { get; private set; }
        public int? PageSize { get; set; } = null;
        public int? PageIndex { get; set; } = null;
        public int? Length { get; set; } = null;
        public bool Status { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string>? ErrorList { get; set; } = null;
        public string? Exception { get; set; } = null;
        public int? StatusCode { get; set; } // Make the set accessor public
        [JsonIgnore]
        private int statusCode { get; set; }
        public CustomJsonResult(T data, int status = StatusCodes.Status200OK, string? errorMessage = null, List<string>? errorList = null, string exception = null)
        {
            Data = data;
            Status = status == StatusCodes.Status200OK ? true : false;
            ErrorMessage = errorMessage;
            ErrorList = errorList;
            Exception = exception;
            statusCode = status;
        }
        public CustomJsonResult(T data, int pageSize, int pageIndex, int length, int status = StatusCodes.Status200OK, string errorMessage = null)
        {
            Length = length;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data;
            Status = status == StatusCodes.Status200OK ? true : false; ;
            ErrorMessage = errorMessage;
            statusCode = status;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = new
            {
                status = Status,
                data = Data,
                errorMessage = ErrorMessage,
                errors = ErrorList,
                length = Length,
                pageIndex = PageIndex,
                pageSize = PageSize
            };
            context.HttpContext.Response.StatusCode = statusCode;
            var result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            await result.ExecuteResultAsync(context);
        }
    }

    public enum NotifyType
    {
        Info,
        Warning,
        Error,
        Success
    }
}
