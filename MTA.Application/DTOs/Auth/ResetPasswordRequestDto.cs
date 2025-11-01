namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Request payload for resetting a password by providing the current and desired values.
/// </summary>
public class ResetPasswordRequestDto
{
    /// <summary>
    /// Current password that will be verified before updating.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password that should replace the current password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password to guard against typos.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
