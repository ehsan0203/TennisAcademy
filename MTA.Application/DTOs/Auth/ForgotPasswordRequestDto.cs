namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Request payload for triggering a password reset email.
/// </summary>
public class ForgotPasswordRequestDto
{
    /// <summary>
    /// Account email address that should receive the reset password.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
