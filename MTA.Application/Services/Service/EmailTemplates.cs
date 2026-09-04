using System.Net;

namespace MTA.Application.Services.Service;

public static class EmailTemplates
{
    private const string Wrapper = """
        <div style="font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; padding: 24px;">
            <h2 style="color: #1a1a1a;">MTA Tennis Academy</h2>
            {0}
            <p style="color: #999; font-size: 12px; margin-top: 32px;">
                If you didn't expect this email, you can safely ignore it.
            </p>
        </div>
        """;

    public static string Welcome(string firstName)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        return string.Format(Wrapper, $"""
            <p>Hi {safeName},</p>
            <p>Welcome to MTA Tennis Academy! Your account has been created successfully.</p>
            <p>You can now log in and start exploring our courses and training plans.</p>
            """);
    }

    public static string PasswordReset(string firstName, string resetLink)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeLink = WebUtility.HtmlEncode(resetLink);
        return string.Format(Wrapper, $"""
            <p>Hi {safeName},</p>
            <p>We received a request to reset your password. Click the button below to choose a new one:</p>
            <p style="text-align: center; margin: 24px 0;">
                <a href="{safeLink}" style="background: #1a1a1a; color: #fff; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;">
                    Reset Password
                </a>
            </p>
            <p>This link expires in 1 hour. If you didn't request this, no action is needed.</p>
            """);
    }

    public static string PackagePurchase(string firstName, string packageTitle, decimal price, int remainingTickets, DateTime expiredDate)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeTitle = WebUtility.HtmlEncode(packageTitle);
        // A free plan is still worth confirming, but telling someone their payment
        // succeeded when they never paid anything reads like a billing mistake.
        var isFree = price <= 0;
        var opening = isFree
            ? $"Your free plan <strong>{safeTitle}</strong> is now active."
            : $"Thank you for your purchase! Your payment for <strong>{safeTitle}</strong> was successful.";
        var amountRow = isFree
            ? "<tr><td style=\"padding: 6px 0; color: #666;\">Price</td><td style=\"padding: 6px 0; text-align: right;\">Free</td></tr>"
            : $"<tr><td style=\"padding: 6px 0; color: #666;\">Amount paid</td><td style=\"padding: 6px 0; text-align: right;\">${price:0.00}</td></tr>";
        return string.Format(Wrapper, $"""
            <p>Hi {safeName},</p>
            <p>{opening}</p>
            <table style="width: 100%; border-collapse: collapse; margin: 16px 0;">
                {amountRow}
                <tr><td style="padding: 6px 0; color: #666;">Remaining tickets</td><td style="padding: 6px 0; text-align: right;">{remainingTickets}</td></tr>
                <tr><td style="padding: 6px 0; color: #666;">Valid until</td><td style="padding: 6px 0; text-align: right;">{expiredDate:MMM d, yyyy}</td></tr>
            </table>
            <p>You can view your credit and chat with a coach anytime from your dashboard.</p>
            """);
    }

    public static string CoursePurchase(string firstName, string courseTitle, decimal price)
    {
        var safeName = WebUtility.HtmlEncode(firstName);
        var safeTitle = WebUtility.HtmlEncode(courseTitle);
        var isFree = price <= 0;
        var opening = isFree
            ? $"You now have access to <strong>{safeTitle}</strong>."
            : $"Thank you for your purchase! You now have access to <strong>{safeTitle}</strong>.";
        return string.Format(Wrapper, $"""
            <p>Hi {safeName},</p>
            <p>{opening}</p>
            <p>{(isFree ? "Price: Free" : $"Amount paid: ${price:0.00}")}</p>
            <p>You can start the course anytime from your dashboard.</p>
            """);
    }
}
