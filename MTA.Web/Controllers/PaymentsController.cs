using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTA.Application.DTOs;
using MTA.Application.Services;
using MTA.Web.Models;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPackageHistoryService _packageHistoryService;
    private readonly IUserCourseHistoryService _userCourseHistoryService;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IConfiguration _config;

    public PaymentsController(
        IPaymentService paymentService,
        IPackageHistoryService packageHistoryService,
        IUserCourseHistoryService userCourseHistoryService,
        ILogger<PaymentsController> logger,
        IConfiguration config)
    {
        _paymentService = paymentService;
        _packageHistoryService = packageHistoryService;
        _userCourseHistoryService = userCourseHistoryService;
        _logger = logger;
        _config = config;
    }

    [HttpPost("package/{packageId}/link")]
    [ProducesResponseType(typeof(CustomJsonResult<PaymentLinkResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    public async Task<CustomJsonResult<PaymentLinkResponseDto>> CreatePackagePaymentLink(
        int packageId, [FromBody] PaymentInitRequestDto request, CancellationToken ct)
    {
        var link = await _paymentService.CreatePackagePaymentLinkAsync(request.AccountId, packageId, request.SuccessUrl, request.CancelUrl, ct);
        return CustomJsonResult<PaymentLinkResponseDto>.SuccessResult(link);
    }

    [HttpGet("square/diagnostics/location")]
    [ProducesResponseType(typeof(CustomJsonResult<LocationDiagnosticsDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<LocationDiagnosticsDto>> SquareLocationDiagnostics(CancellationToken ct)
    {
        var result = await _paymentService.VerifyLocationOwnershipAsync(ct);
        return CustomJsonResult<LocationDiagnosticsDto>.SuccessResult(result);
    }

    [HttpPost("course/{courseId}/link")]
    [ProducesResponseType(typeof(CustomJsonResult<PaymentLinkResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status409Conflict)]
    public async Task<CustomJsonResult<PaymentLinkResponseDto>> CreateCoursePaymentLink(
        int courseId, [FromBody] PaymentInitRequestDto request, CancellationToken ct)
    {
        var alreadyHas = await _userCourseHistoryService.UserHasPurchasedCourseAsync(request.AccountId, courseId, ct);
        if (alreadyHas)
            return CustomJsonResult<PaymentLinkResponseDto>.Conflict("User already owns this course");

        var link = await _paymentService.CreateCoursePaymentLinkAsync(request.AccountId, courseId, request.SuccessUrl, request.CancelUrl, ct);
        return CustomJsonResult<PaymentLinkResponseDto>.SuccessResult(link);
    }

    [HttpGet("result")]
    [ProducesResponseType(typeof(CustomJsonResult<PaymentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomJsonResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<CustomJsonResult<PaymentResultDto>> CheckoutResult(
        [FromQuery(Name = "ref")] string referenceId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(referenceId))
            return CustomJsonResult<PaymentResultDto>.Failure("Missing ref", StatusCodes.Status400BadRequest);

        var parsed = ParseReference(referenceId);
        if (parsed == null)
            return CustomJsonResult<PaymentResultDto>.SuccessResult(new PaymentResultDto { ReferenceId = referenceId, Status = "invalid" });

        var (type, itemId, accountId) = parsed.Value;
        var result = new PaymentResultDto { ReferenceId = referenceId, Type = type, ItemId = itemId, AccountId = accountId, Status = "pending" };

        if (type.Equals("package", StringComparison.OrdinalIgnoreCase))
            result.Status = await _packageHistoryService.UserHasActivePackageAsync(accountId, itemId, ct) ? "success" : "pending";
        else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
            result.Status = await _userCourseHistoryService.UserHasPurchasedCourseAsync(accountId, itemId, ct) ? "success" : "pending";

        return CustomJsonResult<PaymentResultDto>.SuccessResult(result);
    }

    [HttpGet("square/complete")]
    [ProducesResponseType(typeof(CustomJsonResult<PaymentResultDto>), StatusCodes.Status200OK)]
    public async Task<CustomJsonResult<PaymentResultDto>> CompleteSquareOrder([FromQuery] string orderId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return CustomJsonResult<PaymentResultDto>.Failure("Missing orderId", StatusCodes.Status400BadRequest);

        var orderInfo = await _paymentService.GetOrderInfoAsync(orderId, ct);
        if (orderInfo == null || string.IsNullOrWhiteSpace(orderInfo.ReferenceId))
            return CustomJsonResult<PaymentResultDto>.SuccessResult(new PaymentResultDto { ReferenceId = string.Empty, Status = "pending" });

        var parsed = ParseReference(orderInfo.ReferenceId);
        if (parsed == null)
            return CustomJsonResult<PaymentResultDto>.SuccessResult(new PaymentResultDto { ReferenceId = orderInfo.ReferenceId, Status = "invalid" });

        var (type, itemId, accountId) = parsed.Value;
        var result = new PaymentResultDto { ReferenceId = orderInfo.ReferenceId, Type = type, ItemId = itemId, AccountId = accountId, Status = "pending" };

        if (!orderInfo.IsPaid)
            return CustomJsonResult<PaymentResultDto>.SuccessResult(result);

        await HandleSuccessfulReferenceAsync(orderInfo.ReferenceId, ct);

        if (type.Equals("package", StringComparison.OrdinalIgnoreCase))
            result.Status = await _packageHistoryService.UserHasActivePackageAsync(accountId, itemId, ct) ? "success" : "pending";
        else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
            result.Status = await _userCourseHistoryService.UserHasPurchasedCourseAsync(accountId, itemId, ct) ? "success" : "pending";

        return CustomJsonResult<PaymentResultDto>.SuccessResult(result);
    }

    // Webhook: returns IActionResult (not CustomJsonResult) because Square expects plain 200/401
    [HttpPost("square/webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> SquareWebhook(CancellationToken ct)
    {
        var signatureKey = _config["Square:WebhookSignatureKey"] ?? string.Empty;
        var webhookUrl = _config["Square:WebhookUrl"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(signatureKey) || string.IsNullOrWhiteSpace(webhookUrl))
        {
            _logger.LogInformation("Square webhooks disabled or not configured; ignoring webhook call.");
            return Ok();
        }

        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;

        var headerSig = Request.Headers["x-square-hmacsha256-signature"].FirstOrDefault()
                        ?? Request.Headers["x-square-signature"].FirstOrDefault();

        var computed = _paymentService.ComputeWebhookSignature(signatureKey, webhookUrl, body);
        if (string.IsNullOrEmpty(headerSig) || !string.Equals(headerSig, computed, StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid Square webhook signature");
            return Unauthorized();
        }

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        var eventType = root.GetProperty("type").GetString();

        if (eventType != null && eventType.StartsWith("payment.", StringComparison.OrdinalIgnoreCase))
        {
            var payment = root.GetProperty("data").GetProperty("object").GetProperty("payment");
            var status = payment.GetProperty("status").GetString();
            if (string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                var orderId = payment.TryGetProperty("order_id", out var orderEl) ? orderEl.GetString() : null;
                string? referenceId = null;
                if (!string.IsNullOrEmpty(orderId))
                    referenceId = await _paymentService.GetOrderReferenceIdAsync(orderId!, ct);

                if (!string.IsNullOrWhiteSpace(referenceId))
                {
                    _logger.LogInformation("Square payment completed. Order={OrderId}, Ref={ReferenceId}", orderId, referenceId);
                    await HandleSuccessfulReferenceAsync(referenceId!, ct);
                }
            }
        }

        return Ok();
    }

    private async Task HandleSuccessfulReferenceAsync(string referenceId, CancellationToken ct)
    {
        var parsed = ParseReference(referenceId);
        if (parsed == null) return;

        var (type, itemId, accountId) = parsed.Value;

        if (type.Equals("package", StringComparison.OrdinalIgnoreCase))
        {
            await _packageHistoryService.CreateAsync(new CreatePackageHistoryDto { PackageId = itemId, AccountId = accountId }, ct);
        }
        else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
        {
            var purchased = await _userCourseHistoryService.UserHasPurchasedCourseAsync(accountId, itemId, ct);
            if (!purchased)
                await _userCourseHistoryService.CreateAsync(new CreateUserCourseHistoryDto { CourseId = itemId, AccountId = accountId }, ct);
        }
    }

    private static (string type, int itemId, int accountId)? ParseReference(string referenceId)
    {
        var parts = referenceId.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 4 && parts[2].Equals("account", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(parts[1], out var itemId) && int.TryParse(parts[3], out var accountId))
            return (parts[0], itemId, accountId);
        return null;
    }
}
