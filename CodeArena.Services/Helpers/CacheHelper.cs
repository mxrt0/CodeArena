using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Helpers;

public static class CacheHelper
{
    public static void Remove(IMemoryCache cache, params string[] keys)
    {
        foreach (var key in keys)
            cache.Remove(key);
    }
}
