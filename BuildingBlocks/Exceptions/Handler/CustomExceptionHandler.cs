using BuildingBlocks.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Net.Http;

namespace BuildingBlocks.Exceptions.Handler;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(InternalServerException), HandleInternalServerException },
            { typeof(Exception), UnHandleException },
            { typeof(NullReferenceException), HandleNullReferenceException },
            { typeof(ApiException), ApiExceptionHandler },
            { typeof(BadRequestException), BadRequestExceptionHandler }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;
        var errors = exception.Errors.SelectMany(c => c.Value.Select(e => e).ToList()).ToList();
        Log.Error("ValidationException: {Message}, Errors: {Errors}, Time: {Time}, TraceId: {TraceId}, StackTrace: {StackTrace}",
                  exception?.Message, errors, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString(), exception?.StackTrace);

        var details = new CustomJsonResult<string>(null, StatusCodes.Status400BadRequest, "مقادیر مورد نیاز بدرستی وارد نشده اند.", errors, exception.Message);
        context.Result = new BadRequestObjectResult(details)
        {
            StatusCode = 400
        };
        context.ExceptionHandled = true;
    }

    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        Log.Error("InvalidModelStateException: {Message}, Time: {Time}, TraceId: {TraceId}, StackTrace: {StackTrace}",
                  context.Exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString(), context.Exception?.StackTrace);

        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        };
        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;
        Log.Error("NotFoundException: {Message}, Time: {Time}, TraceId: {TraceId}", exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString());

        var details = new CustomJsonResult<string>(null, 404, exception.Message);
        context.Result = new NotFoundObjectResult(details)
        {
            StatusCode = 404
        };
        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var exception = (UnauthorizedAccessException)context.Exception;
        Log.Error("UnauthorizedAccessException: {Message}, Time: {Time}, TraceId: {TraceId}", exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString());

        var details = new CustomJsonResult<string>(null, 401, "خطای دسترسی.", null, exception?.Message);
        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        var exception = (ForbiddenAccessException)context.Exception;
        Log.Error("ForbiddenAccessException: {Message}, Time: {Time}, TraceId: {TraceId}", exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString());

        var details = new CustomJsonResult<string>(null, 403, string.IsNullOrWhiteSpace(exception?.Message) ? "شما به این بخش دسترسی ندارید." : exception.Message + (exception.ErrorCode != null ? $"کد خطا:{exception.ErrorCode}" : ""));
        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
        context.ExceptionHandled = true;
    }

    private void UnHandleException(ExceptionContext context)
    {
        var exception = context.Exception;
        Log.Error("UnhandledException: {Message}, Time: {Time}, TraceId: {TraceId}, StackTrace: {StackTrace}",
                  exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString(), exception?.StackTrace);

        var detail = new CustomJsonResult<string>(null, StatusCodes.Status500InternalServerError, "خطای ناشناخته رخ داده است.", null, exception?.Message);
        context.Result = new ObjectResult(detail)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = false;
    }

    private void HandleInternalServerException(ExceptionContext context)
    {
        var exception = (InternalServerException)context.Exception;
        Log.Error("InternalServerException: {Message}, Time: {Time}, TraceId: {TraceId}, StackTrace: {StackTrace}",
                  exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString(), exception?.StackTrace);

        var detail = new CustomJsonResult<string>(null, StatusCodes.Status500InternalServerError, string.IsNullOrEmpty(exception?.Details) ? "خطای داخلی رخ داده است." : exception.Details, null, exception.Message);
        context.Result = new ObjectResult(detail)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }

    private void HandleNullReferenceException(ExceptionContext context)
    {
        var exception = context.Exception as NullReferenceException;
        Log.Error("NullReferenceException: {Message}, Time: {Time}, TraceId: {TraceId}, StackTrace: {StackTrace}",
                  exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString(), exception?.StackTrace);

        var detail = new CustomJsonResult<string>(null, StatusCodes.Status500InternalServerError, "خطای داخلی رخ داده است.", null, exception?.Message);
        context.Result = new ObjectResult(detail)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }

    private void ApiExceptionHandler(ExceptionContext context)
    {
        var exception = context.Exception as ApiException;
        Log.Error("ApiException: {Message}, Time: {Time}, ErrorCode: {ErrorCode}, TraceId: {TraceId}",
                  exception?.Message, DateTime.UtcNow, exception?.ErrorCode, exception?.TraceId);

        var detail = new CustomJsonResult<string>(null, exception.StatusCode,
            $"{(string.IsNullOrEmpty(exception.Details) ? $"خطا در ارتباط با سرویس میزبان.{(exception.ErrorCode != null ? $"کد خطا:{exception.ErrorCode}" : "")}"
                : exception.Details)} {(exception.ErrorCode != null ? $".کد خطا:{exception.ErrorCode}" : "")}",
            null,
            exception.Message);
        context.Result = new ObjectResult(detail)
        {
            StatusCode = exception.StatusCode
        };
        context.ExceptionHandled = true;
    }

    private void BadRequestExceptionHandler(ExceptionContext context)
    {
        var exception = context.Exception as BadRequestException;
        Log.Error("BadRequestException: {Message}, Time: {Time}, TraceId: {TraceId}",
                  exception?.Exception != null ? exception.Exception.Message : exception?.Message, DateTime.UtcNow, context.HttpContext.Request.Headers["mc-request-id"].ToString());

        var detail = new CustomJsonResult<string>(null, StatusCodes.Status400BadRequest, exception?.Message, null, exception?.Exception != null ? exception.Exception.Message : "");
        context.Result = new ObjectResult(detail)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
        context.ExceptionHandled = true;
    }
}
