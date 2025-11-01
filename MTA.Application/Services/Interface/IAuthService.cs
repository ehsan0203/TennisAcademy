using MTA.Application.DTOs.Auth;

namespace MTA.Application.Services;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerDto">Registration data</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New authentication response</returns>
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>True if successful</returns>
    Task<bool> RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if valid</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Generates a new password for the provided email and sends it through the configured email provider.
    /// </summary>
    /// <param name="requestDto">Payload that contains the target email address.</param>
    /// <returns>True if the password reset process succeeds.</returns>
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto requestDto);

    /// <summary>
    /// Updates the password for the authenticated account.
    /// </summary>
    /// <param name="accountId">Identifier of the authenticated account.</param>
    /// <param name="requestDto">Payload that contains the current and desired password values.</param>
    /// <returns>True if the password is updated successfully.</returns>
    Task<bool> ResetPasswordAsync(int accountId, ResetPasswordRequestDto requestDto);
}
