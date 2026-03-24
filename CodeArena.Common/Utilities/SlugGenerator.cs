using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeArena.Common.Utilities;

public static class SlugGenerator
{
    public static string Generate(string input)
    {
        var slug = input.ToLowerInvariant();

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = slug.Replace(" ", "-");

        return slug;
    }
    public static string GenerateUnique(string title, HashSet<string> existingSlugs)
    {
        var baseSlug = Generate(title);
        var slug = baseSlug;
        int counter = 1;

        while (existingSlugs.Contains(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}
