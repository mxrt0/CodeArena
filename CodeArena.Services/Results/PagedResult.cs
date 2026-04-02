using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Results;

public class PagedResult<T> where T : class
{
    public PagedResult(IEnumerable<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; } 
}
