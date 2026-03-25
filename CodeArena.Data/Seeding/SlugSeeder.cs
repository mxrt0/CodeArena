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

        var slugSet = await challengeRepository.GetExistingSlugsAsync();

        if (!challengesWithoutSlugs.Any()) return;

        foreach (var c in challengesWithoutSlugs)
        {
            var slug = SlugGenerator.GenerateUnique(c.Title, slugSet);

            c.Slug = slug;
            slugSet.Add(slug);
        }

        await challengeRepository.SaveChangesAsync();
    }
}
