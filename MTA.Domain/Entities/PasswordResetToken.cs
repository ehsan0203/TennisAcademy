namespace MTA.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
