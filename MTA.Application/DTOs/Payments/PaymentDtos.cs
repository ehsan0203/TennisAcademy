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
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// True when the item costs nothing and was handed to the account directly.
    /// There is no Url to redirect to in that case — Square cannot take a $0 payment,
    /// it leaves the order unpaid forever, and the buyer never gets what they claimed.
    /// </summary>
    public bool Granted { get; set; }
}

public class PaymentResultDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending"; // success | pending | invalid
    public string Type { get; set; } = string.Empty; // package | course
    public int AccountId { get; set; }
    public int ItemId { get; set; }
}

public class PaymentOrderInfoDto
{
    public string OrderId { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty; // Square order state
    public bool IsPaid { get; set; }
}
