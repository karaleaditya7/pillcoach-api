namespace OntrackDb.Filter
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Name { get; set; }
        public int TotalCount { get; set; }

    }
}
