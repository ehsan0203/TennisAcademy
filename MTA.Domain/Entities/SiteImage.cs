namespace MTA.Domain.Entities
{
    public class SiteImage : BaseEntity
    {
        public string Key { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}
