using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin;

public class SlugGeneratorService : ISlugGeneratorService
{
    private readonly IChallengeRepository _challengeRepository;

    public SlugGeneratorService(IChallengeRepository challengeRepository)
    {
        _challengeRepository = challengeRepository;
    }

    public async Task<string> GenerateUniqueSlugAsync(string input)
    {
        var slug = input.ToLowerInvariant();

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", " ").Trim();
        slug = slug.Replace(" ", "-");

        int counter = 1;
        while (await _challengeRepository.AnyAsync(c => c.Slug == slug))
        {
            slug = $"{slug}-{counter}";
            counter++;
        }

        return slug;

    }
}
