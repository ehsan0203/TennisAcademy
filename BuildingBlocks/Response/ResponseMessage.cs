using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Response
{
    public class ResponseMessage<T>
    {

        public bool Status { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string>? Errors { get; set; }
        public int? Length { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }

    public class ErrorResponse
    {
        public string title { get; set; }
        public int status { get; set; }
        public string detail { get; set; }
        public string message { get; set; }
        public string instance { get; set; }
        public string traceId { get; set; }
        public int errorCode { get; set; }
        public ValidationErrors? ValidationErrors { get; set; }
    }
    public class ValidationErrors : Dictionary<string, List<string>>
    {
    }
}
