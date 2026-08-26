namespace MTA.Domain.Entities
{
    public class FooterContactItem : BaseEntity
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
