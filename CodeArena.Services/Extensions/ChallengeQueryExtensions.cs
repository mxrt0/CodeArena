using CodeArena.Data.Models;
using CodeArena.Services.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Extensions;

public static class ChallengeQueryExtensions
{
    public static IQueryable<Challenge> ApplyFiltering(
        this IQueryable<Challenge> query,
        ChallengeQuery filter)
    {
        query = query.OrderBy(c => c.Id);

        if (filter.Tags is not null && filter.Tags.Any())
        {
            var normalizedTags = filter.Tags
                                        .Select(t => t.Trim().ToLower())
                                        .ToList();

            query = query.Where(c =>
                normalizedTags.Any(tag =>
                    ("," + c.Tags.ToLower() + ",").Contains("," + tag + ",")
                )
            );
        }

        if (filter.Difficulty is not null)
        {
            query = query.Where(c => c.Difficulty == filter.Difficulty);
        }

        filter.Search = filter.Search?.Trim();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search;
            query = query.Where(c => c.Title.Contains(search));
        }

        return query;
    }
    
}
