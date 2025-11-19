using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IPaymentService
{
    Task<PaymentLinkResponseDto> CreatePackagePaymentLinkAsync(int accountId, int packageId, string successUrl, string? cancelUrl);
    Task<PaymentLinkResponseDto> CreateCoursePaymentLinkAsync(int accountId, int courseId, string successUrl, string? cancelUrl);

    Task<string?> GetOrderReferenceIdAsync(string orderId);
    string ComputeWebhookSignature(string signatureKey, string notificationUrl, string body);

    Task<LocationDiagnosticsDto> VerifyLocationOwnershipAsync();
}
