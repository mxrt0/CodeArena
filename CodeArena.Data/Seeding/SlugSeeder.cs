using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArena.Common.Utilities;
using CodeArena.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Data.Seeding;

public static class SlugSeeder
{
    public static async Task EnsureAllChallengesHaveSlugsAsync(IChallengeRepository challengeRepository)
    {
        var challengesWithoutSlugs = await challengeRepository
                                        .GetAllTracked()
                                        .Where(c => string.IsNullOrWhiteSpace(c.Slug))
                                        .ToListAsync();

        var existingSlugs = await challengeRepository
                                        .GetAll()
                                        .Where(c => !string.IsNullOrWhiteSpace(c.Slug))
                                        .Select(c => c.Slug)
                                        .ToListAsync();

        var slugSet = new HashSet<string>(existingSlugs);

        if (!challengesWithoutSlugs.Any()) return;

        foreach (var c in challengesWithoutSlugs)
        {
            var baseSlug = SlugGenerator.Generate(c.Title);
            var slug = baseSlug;
            int counter = 1;

            while (slugSet.Contains(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            c.Slug = slug;
            slugSet.Add(slug);
        }

        await challengeRepository.SaveChangesAsync();
    }
}
