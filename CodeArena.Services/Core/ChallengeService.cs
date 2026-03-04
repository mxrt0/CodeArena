using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Challenge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core;

public class ChallengeService : IChallengeService
{
    private readonly IChallengeRepository _repository;

    public ChallengeService(IChallengeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChallengeDisplayDto?> GetChallengeByIdAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id);
        if (challenge is null)
        {
            return null;
        }

        return new ChallengeDisplayDto(
            challenge.Id,
            challenge.Title,
            challenge.Description,
            challenge.Difficulty.ToString(),
            challenge.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray(),
            challenge.Submissions.Count
        );
    }

    public async Task<IEnumerable<ChallengeDisplayDto>> GetChallengesAsync()
    {
        var challenges = _repository.GetAll();

        return await challenges.Select(c => new ChallengeDisplayDto(
                c.Id,
                c.Title,
                c.Description,
                c.Difficulty.ToString(),
                c.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray(),
                c.Submissions.Count
        )).ToListAsync();
    }
}
