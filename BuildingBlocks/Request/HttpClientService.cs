using BuildingBlocks.Exceptions;
using BuildingBlocks.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace BuildingBlocks.Request
{
    public interface IHttpClientService
    {
        Task<HttpResponseWrapper<TResponse>> SendAsync<TRequest, TResponse>(HttpRequestConfig<TRequest> config, CancellationToken cancellationToken = default);
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly SignRequest _signRequest;


        public HttpClientService(IHttpClientFactory httpClientFactory,
                                 IConfiguration configuration,
                                 SignRequest signRequest)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _clientId = configuration["Host:ClientId"] ?? throw new NotSetConfigException("ClientId Not Found");
            _signRequest = signRequest;
        }

        public async Task<HttpResponseWrapper<TResponse>> SendAsync<TRequest, TResponse>(HttpRequestConfig<TRequest> config, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("HubService");

            Log.Information("Sending HTTP {Method} request to {Url}", config.Method, config.Url);

            return await SendRequestAsync<TRequest, TResponse>(client, config, cancellationToken);
        }

        private async Task<HttpResponseWrapper<TResponse>> SendRequestAsync<TRequest, TResponse>(HttpClient client, HttpRequestConfig<TRequest> config, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(config.Method, config.Url);
            if (config.Body != null)
            {
                var customerCodeProp = typeof(TRequest).GetProperty("CustomerCode");
                if (customerCodeProp != null && customerCodeProp.CanWrite)
                {
                    customerCodeProp.SetValue(config.Body, int.Parse(_clientId));
                }
            }

            if (config.Headers is not null)
            {
                config.Headers.Add("X-Client-ID", _clientId);
                config.Headers.Add("X-Signature", _signRequest.SignData(config.Message + _clientId));
                config.Headers.Add("X-Message", Helper.Helper.ConvertToBase64(config.Message + _clientId));
            }
            else
            {
                config.Headers = new Dictionary<string, string>()
                {
                    { "X-Client-ID", _clientId },
                    { "X-Signature", _signRequest.SignData(config.Message + _clientId) },
                    { "X-Message", Helper.Helper.ConvertToBase64(config.Message + _clientId) }
                };
            }



            foreach (var header in config.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(config.MediaType));

            if (config.Body != null)
            {
                if (config.MediaType == "application/json")
                {
                    var json = JsonConvert.SerializeObject(config.Body);
                    request.Content = new StringContent(json, Encoding.UTF8, config.MediaType);
                    var contentBytes = Encoding.UTF8.GetByteCount(json);
                    Log.Information("Request body size: {Size} bytes", contentBytes);

                }
                else if (config.MediaType == "application/x-www-form-urlencoded" && config.Body is Dictionary<string, string> formData)
                {
                    request.Content = new FormUrlEncodedContent(formData);
                }
                else
                {
                    return new HttpResponseWrapper<TResponse>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Unsupported media type or body format.",
                        StatusCode = 0
                    };
                }
            }
            var response = await client.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    Log.Information("Response Message : {Value}", responseContent);

                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                    if (errorResponse?.ValidationErrors is not null && errorResponse.ValidationErrors.Any())
                    {
                        var validationLog = new StringBuilder();
                        foreach (var field in errorResponse.ValidationErrors)
                        {
                            var errors = string.Join(", ", field.Value);
                            validationLog.AppendLine($"{field.Key}: {errors}");
                        }
                        Log.Error("Validation Error : Title {Title} - Value: {Value}", errorResponse.title, validationLog.ToString());
                    }

                    throw new ApiException(
                        errorResponse?.status ?? (int)response.StatusCode,
                        errorResponse?.message ?? "ارتباط با سرویس میزبان برقرار نشد.",
                        errorResponse?.errorCode ?? 0,
                        errorResponse?.detail,
                        errorResponse?.traceId);
                }
                catch (JsonReaderException ex)
                {
                    Log.Error(ex, "Invalid JSON format. Content: {Content}", responseContent);
                    throw new ApiException(
                        (int)response.StatusCode,
                        responseContent,
                        0,
                        "مشکل در تبدیل پاسخ دریافتی.");
                }
                catch (JsonSerializationException ex)
                {
                    Log.Error(ex, "Error deserializing JSON to ErrorResponse. Content: {Content}", responseContent);
                    throw new ApiException(
                        (int)response.StatusCode,
                        responseContent,
                        0,
                        "مشکل در تبدیل پاسخ دریافتی.");
                }
                catch (InternalServerException e)
                {
                    throw new InternalServerException($"DeserializeObject error: {e.Message}");
                }

            }
            try
            {

                var responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);
                return new HttpResponseWrapper<TResponse>
                {
                    IsSuccess = true,
                    Data = responseObject,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                throw new InternalServerException($"DeserializeObject error: {ex.Message}");
            }
        }
    }

    public class HttpRequestConfig<TRequest>
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public TRequest? Body { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string MediaType { get; set; } = "application/json";
        public string Message { get; set; }
    }

    public class HttpResponseWrapper<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
}