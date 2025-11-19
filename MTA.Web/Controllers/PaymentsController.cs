using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Application.Services;

namespace MTA.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<PaymentLinkResponseDto>> CreatePackagePaymentLink(int packageId, [FromBody] PaymentInitRequestDto request)
    {
        try
        {
            var link = await _paymentService.CreatePackagePaymentLinkAsync(request.AccountId, packageId, request.SuccessUrl, request.CancelUrl);
            return Ok(link);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating package payment link");
            return StatusCode(500, "Failed to create payment link");
        }
    }

    [HttpGet("square/diagnostics/location")]
    public async Task<ActionResult<LocationDiagnosticsDto>> SquareLocationDiagnostics()
    {
        try
        {
            var result = await _paymentService.VerifyLocationOwnershipAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running Square location diagnostics");
            return StatusCode(500, new { ok = false, message = "Diagnostics failed" });
        }
    }

    [HttpPost("course/{courseId}/link")]
    public async Task<ActionResult<PaymentLinkResponseDto>> CreateCoursePaymentLink(int courseId, [FromBody] PaymentInitRequestDto request)
    {
        try
        {
            var link = await _paymentService.CreateCoursePaymentLinkAsync(request.AccountId, courseId, request.SuccessUrl, request.CancelUrl);
            return Ok(link);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course payment link");
            return StatusCode(500, "Failed to create payment link");
        }
    }

    // Square redirects here with ?ref=... after checkout completes (client-side only)
    // This action lets your frontend verify current status based on the webhook side-effects.
    [HttpGet("result")]
    public async Task<ActionResult<PaymentResultDto>> CheckoutResult([FromQuery(Name = "ref")] string referenceId)
    {
        if (string.IsNullOrWhiteSpace(referenceId))
        {
            return BadRequest("Missing ref");
        }

        var parsed = ParseReference(referenceId);
        if (parsed == null)
        {
            return Ok(new PaymentResultDto
            {
                ReferenceId = referenceId,
                Status = "invalid",
            });
        }

        var (type, itemId, accountId) = parsed.Value;

        var result = new PaymentResultDto
        {
            ReferenceId = referenceId,
            Type = type,
            ItemId = itemId,
            AccountId = accountId,
            Status = "pending"
        };

        try
        {
            if (type.Equals("package", StringComparison.OrdinalIgnoreCase))
            {
                var hasActive = await _packageHistoryService.UserHasActivePackageAsync(accountId, itemId);
                result.Status = hasActive ? "success" : "pending";
            }
            else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
            {
                var purchased = await _userCourseHistoryService.UserHasPurchasedCourseAsync(accountId, itemId);
                result.Status = purchased ? "success" : "pending";
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment result for {ReferenceId}", referenceId);
            // Do not leak internal errors; keep a graceful pending result
            return Ok(result);
        }
    }

    [HttpGet("square/webhook")]
    public async Task<IActionResult> SquareWebhook()
    {
        // Verify signature
        var signatureKey = _config["Square:WebhookSignatureKey"] ?? string.Empty;
        var webhookUrl = _config["Square:WebhookUrl"] ?? string.Empty;
        // Webhooks disabled or not configured: acknowledge and exit gracefully
        if (string.IsNullOrWhiteSpace(signatureKey) || string.IsNullOrWhiteSpace(webhookUrl))
        {
            _logger.LogInformation("Square webhooks disabled or not configured; ignoring webhook call.");
            return Ok();
        }

        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var headerSig = Request.Headers["x-square-hmacsha256-signature"].FirstOrDefault()
                        ?? Request.Headers["x-square-signature"].FirstOrDefault();

        var computed = _paymentService.ComputeWebhookSignature(signatureKey, webhookUrl, body);
        if (string.IsNullOrEmpty(headerSig) || !string.Equals(headerSig, computed, StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid Square webhook signature");
            return Unauthorized();
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var eventType = root.GetProperty("type").GetString();

            // We're interested in completed payments
            if (eventType != null && eventType.StartsWith("payment.", StringComparison.OrdinalIgnoreCase))
            {
                var payment = root.GetProperty("data").GetProperty("object").GetProperty("payment");
                var status = payment.GetProperty("status").GetString();
                if (string.Equals(status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    var orderId = payment.TryGetProperty("order_id", out var orderEl) ? orderEl.GetString() : null;
                    string? referenceId = null;
                    if (!string.IsNullOrEmpty(orderId))
                    {
                        referenceId = await _paymentService.GetOrderReferenceIdAsync(orderId!);
                    }

                    if (!string.IsNullOrWhiteSpace(referenceId))
                    {
                        _logger.LogInformation("Square payment completed. Order={OrderId}, Ref={ReferenceId}", orderId, referenceId);
                        await HandleSuccessfulReferenceAsync(referenceId!);
                    }
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Square webhook");
            return StatusCode(500);
        }
    }

    private async Task HandleSuccessfulReferenceAsync(string referenceId)
    {
        var parsed = ParseReference(referenceId);
        if (parsed == null) return;

        var (type, itemId, accountId) = parsed.Value;

        if (type.Equals("package", StringComparison.OrdinalIgnoreCase))
        {
            await _packageHistoryService.CreateAsync(new CreatePackageHistoryDto
            {
                PackageId = itemId,
                AccountId = accountId,
                IsExpired = false
            });
        }
        else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
        {
            await _userCourseHistoryService.CreateAsync(new CreateUserCourseHistoryDto
            {
                CourseId = itemId,
                AccountId = accountId
            });
        }
    }

    private static (string type, int itemId, int accountId)? ParseReference(string referenceId)
    {
        // Expected formats:
        // package:{packageId}:account:{accountId}
        // course:{courseId}:account:{accountId}
        var parts = referenceId.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 4 && parts[2].Equals("account", StringComparison.OrdinalIgnoreCase))
        {
            var type = parts[0];
            if (int.TryParse(parts[1], out var itemId) && int.TryParse(parts[3], out var accountId))
            {
                return (type, itemId, accountId);
            }
        }
        return null;
    }
}
