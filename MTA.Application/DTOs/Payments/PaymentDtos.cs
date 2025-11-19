namespace MTA.Application.DTOs;

public class PaymentInitRequestDto
{
    public int AccountId { get; set; }
    public string SuccessUrl { get; set; } = string.Empty;
    public string? CancelUrl { get; set; }
}

public class PaymentLinkResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string LinkId { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
}

public class PaymentResultDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending"; // success | pending | invalid
    public string Type { get; set; } = string.Empty; // package | course
    public int AccountId { get; set; }
    public int ItemId { get; set; }
}
