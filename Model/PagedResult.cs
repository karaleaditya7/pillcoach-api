using System.Collections.Generic;

namespace OntrackDb.Model;

public class PagedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPageCount { get; set; }
    public int TotalRecordCount { get; set; }
    public IEnumerable<T> Records { get; set; }
}
