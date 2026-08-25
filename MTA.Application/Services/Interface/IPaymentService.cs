using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IPaymentService
{
    Task<PaymentLinkResponseDto> CreatePackagePaymentLinkAsync(int accountId, int packageId, string successUrl, string? cancelUrl, CancellationToken ct = default);
    Task<PaymentLinkResponseDto> CreateCoursePaymentLinkAsync(int accountId, int courseId, string successUrl, string? cancelUrl, CancellationToken ct = default);
    Task<string?> GetOrderReferenceIdAsync(string orderId, CancellationToken ct = default);
    Task<PaymentOrderInfoDto?> GetOrderInfoAsync(string orderId, CancellationToken ct = default);
    string ComputeWebhookSignature(string signatureKey, string notificationUrl, string body);
    Task<LocationDiagnosticsDto> VerifyLocationOwnershipAsync(CancellationToken ct = default);
}
