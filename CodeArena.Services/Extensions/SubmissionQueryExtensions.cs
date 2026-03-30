using CodeArena.Data.Models;
using CodeArena.Services.QueryModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Extensions;

public static class SubmissionQueryExtensions
{
    public static IQueryable<Submission> ApplyFiltering(
        this IQueryable<Submission> query,
        SubmissionQuery filter)
    {
        query = query.OrderBy(s => s.Id);

        if (filter.Language.HasValue)
        {
            query = query.Where(s => s.Language == filter.Language.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(s => s.Status == filter.Status.Value);
        }

        return query;
    }
}
