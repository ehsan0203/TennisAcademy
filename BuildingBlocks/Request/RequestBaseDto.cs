namespace BuildingBlocks.Request
{
    public record RequestBaseDto
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string? Search { get; set; } = null;
        public bool SortAsDate { get; set; } = true;
        public bool SortAsName { get; set; } = true;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
