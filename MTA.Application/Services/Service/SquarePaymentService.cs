using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Square.Apis;
using Square.Models;
using Square.Exceptions;

namespace MTA.Application.Services;

public class SquarePaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SquarePaymentService> _logger;
    private readonly IConfiguration _config;

    private readonly string _accessToken;
    private readonly string _applicationId;
    private readonly string _environment;
    private readonly string _locationId;
    private readonly string _currency;

    public SquarePaymentService(
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory,
        ILogger<SquarePaymentService> logger,
        IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;

        _accessToken = _config["Square:AccessToken"] ?? string.Empty;
        _applicationId = _config["Square:ApplicationId"] ?? string.Empty;
        _environment = _config["Square:Environment"] ?? "Sandbox";
        _locationId = _config["Square:LocationId"] ?? string.Empty;
        _currency = _config["Square:Currency"] ?? "USD";
    }

    private string BaseUrl => _environment.Equals("Sandbox", StringComparison.OrdinalIgnoreCase)
        ? "https://connect.squareupsandbox.com"
        : "https://connect.squareup.com";

    public async Task<PaymentLinkResponseDto> CreatePackagePaymentLinkAsync(int accountId, int packageId, string successUrl, string? cancelUrl)
    {
        var package = await _unitOfWork.Repository<Package>().GetByIdAsync(packageId)
            ?? throw new KeyNotFoundException("Package not found");

        var referenceId = $"package:{packageId}:account:{accountId}";
        var name = package.Title ?? $"Package #{packageId}";
        var amountCents = (long)(package.Price * 100m);

        return await CreatePaymentLinkInternalAsync(name, amountCents, referenceId, successUrl, cancelUrl);
    }

    public async Task<PaymentLinkResponseDto> CreateCoursePaymentLinkAsync(int accountId, int courseId, string successUrl, string? cancelUrl)
    {
        var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseId)
            ?? throw new KeyNotFoundException("Course not found");

        var referenceId = $"course:{courseId}:account:{accountId}";
        var name = course.Title ?? $"Course #{courseId}";
        var amountCents = (long)(course.Price * 100m);

        return await CreatePaymentLinkInternalAsync(name, amountCents, referenceId, successUrl, cancelUrl);
    }

    private async Task<PaymentLinkResponseDto> CreatePaymentLinkInternalAsync(
        string itemName, long amountCents, string referenceId, string successUrl, string? cancelUrl)
    {
        if (string.IsNullOrWhiteSpace(_accessToken) || string.IsNullOrWhiteSpace(_locationId))
            throw new InvalidOperationException("Square configuration is missing AccessToken or LocationId");
        if (string.IsNullOrWhiteSpace(successUrl))
            throw new ArgumentException("SuccessUrl is required", nameof(successUrl));

        var idempotencyKey = Guid.NewGuid().ToString("N");

        // Append reference id to SuccessUrl so the result page can verify status
        var separator = successUrl.Contains('?') ? '&' : '?';
        var successUrlWithRef = $"{successUrl}{separator}ref={Uri.EscapeDataString(referenceId)}";

        var sqClient = BuildSquareClient();

        var money = new Money(amount: amountCents, currency: _currency);
        var lineItem = new OrderLineItem(quantity: "1",
            name: itemName,
            basePriceMoney: money);

        var order = new Order(locationId: _locationId,
            referenceId: referenceId,
            lineItems: new List<OrderLineItem> { lineItem });

        var checkoutOptions = new CheckoutOptions(redirectUrl: successUrlWithRef);
        var req = new CreatePaymentLinkRequest(
            idempotencyKey: idempotencyKey,
            order: order,
            checkoutOptions: checkoutOptions);

        try
        {
            _logger.LogInformation("Creating Square payment link: Ref={ReferenceId}, Amount={Amount}", referenceId, amountCents);
            var resp = await sqClient.CheckoutApi.CreatePaymentLinkAsync(req);
            var link = resp?.PaymentLink;
            if (link == null)
            {
                _logger.LogError("Square SDK returned no payment link for Ref={ReferenceId}", referenceId);
                throw new InvalidOperationException("Failed to create Square payment link");
            }

            return new PaymentLinkResponseDto
            {
                Url = link.Url ?? string.Empty,
                LinkId = link.Id ?? string.Empty,
                ReferenceId = referenceId,
                OrderId = link.OrderId ?? string.Empty
            };
        }
        catch (ApiException apiEx)
        {
            var body = apiEx.ResponseCode + ": " + string.Join(" | ", apiEx.Errors?.Select(e => $"{e.Category}:{e.Code}:{e.Detail}") ?? Array.Empty<string>());
            _logger.LogError(apiEx, "Square CreatePaymentLink unauthorized or failed: {Body}", body);
            throw new InvalidOperationException("Failed to create Square payment link");
        }
    }

    public async Task<string?> GetOrderReferenceIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId)) return null;

        try
        {
            var sqClient = BuildSquareClient();
            var ordersApi = sqClient.OrdersApi;
            var response = await ordersApi.RetrieveOrderAsync(orderId);
            if (response?.Order == null)
            {
                _logger.LogWarning("Square order not found or empty: {OrderId}", orderId);
                return null;
            }

            return response.Order.ReferenceId;
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx, "Square SDK error retrieving order {OrderId}", orderId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving order {OrderId}", orderId);
            return null;
        }
    }

    public async Task<PaymentOrderInfoDto?> GetOrderInfoAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId)) return null;

        try
        {
            var sqClient = BuildSquareClient();
            var ordersApi = sqClient.OrdersApi;
            var response = await ordersApi.RetrieveOrderAsync(orderId);
            if (response?.Order == null)
            {
                _logger.LogWarning("Square order not found or empty: {OrderId}", orderId);
                return null;
            }

            var state = response.Order.State ?? string.Empty;
            var isPaid = string.Equals(state, "COMPLETED", StringComparison.OrdinalIgnoreCase);

            return new PaymentOrderInfoDto
            {
                OrderId = orderId,
                ReferenceId = response.Order.ReferenceId ?? string.Empty,
                State = state,
                IsPaid = isPaid
            };
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx, "Square SDK error retrieving order {OrderId}", orderId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving order {OrderId}", orderId);
            return null;
        }
    }

    private Square.SquareClient BuildSquareClient()
    {
        var env = _environment.Equals("Sandbox", StringComparison.OrdinalIgnoreCase)
            ? Square.Environment.Sandbox
            : Square.Environment.Production;

        return new Square.SquareClient.Builder()
            .Environment(env)
            .AccessToken(_accessToken)
            .Build();
    }

    public string ComputeWebhookSignature(string signatureKey, string notificationUrl, string body)
    {
        var signingString = notificationUrl + body;
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signatureKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingString));
        return Convert.ToBase64String(hash);
    }

    public async Task<LocationDiagnosticsDto> VerifyLocationOwnershipAsync()
    {
        var result = new LocationDiagnosticsDto
        {
            Environment = _environment,
            ConfiguredLocationId = _locationId,
            Ok = false,
            LocationFound = false
        };

        try
        {
            var client = BuildSquareClient();
            var resp = await client.LocationsApi.ListLocationsAsync();
            var locations = resp?.Locations ?? new List<Location>();

            var match = locations.FirstOrDefault(l => string.Equals(l.Id, _locationId, StringComparison.Ordinal));
            if (match != null)
            {
                result.Ok = true;
                result.LocationFound = true;
                result.MatchedLocationName = match.Name;
                result.Message = "Configured LocationId is valid for current token/environment.";
                _logger.LogInformation("Square diagnostics: location verified for env {Env}, id {LocationId}, name {Name}", _environment, _locationId, match.Name);
            }
            else
            {
                result.Ok = false;
                result.LocationFound = false;
                result.Message = "Configured LocationId not found among locations for current token/environment.";
                _logger.LogError("Square diagnostics: Configured LocationId {LocationId} NOT found for env {Env}. Available locations count: {Count}", _locationId, _environment, locations.Count);
            }
        }
        catch (ApiException apiEx)
        {
            result.Ok = false;
            result.Message = $"Square API error: {apiEx.ResponseCode} - {apiEx.Errors?.FirstOrDefault()?.Detail ?? "Unauthorized or invalid credentials"}";
            _logger.LogError(apiEx, "Square diagnostics API error {Code} in env {Env}", apiEx.ResponseCode, _environment);
        }
        catch (Exception ex)
        {
            result.Ok = false;
            result.Message = "Unexpected error during Square diagnostics.";
            _logger.LogError(ex, "Square diagnostics unexpected error in env {Env}", _environment);
        }

        return result;
    }
}
