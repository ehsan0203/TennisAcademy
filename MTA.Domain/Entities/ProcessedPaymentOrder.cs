namespace MTA.Domain.Entities
{
    public class ProcessedPaymentOrder : BaseEntity
    {
        public string OrderId { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}
