using CodeArena.Data.Models;
using CodeArena.Services.DTOs.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Helpers;

public static class ChallengeMapper
{
    public static ChallengeDisplayDto ToDto(Challenge c)
    {
        return new ChallengeDisplayDto(
                c.Id,
                c.Slug,
                c.Title,
                c.Description,
                c.Difficulty.ToString(),
                c.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray(),
                c.Submissions.Count,
                c.IsDeleted
        );
    }
}
