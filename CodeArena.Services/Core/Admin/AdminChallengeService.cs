using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.DTOs.Challenge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin;

public class AdminChallengeService : IAdminChallengeService
{
    private readonly IChallengeRepository _repository;

    public AdminChallengeService(IChallengeRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateChallengeAsync(CreateChallengeDto dto)
    {
        var challenge = new Challenge
        {
            Title = dto.Title,
            Description = dto.Description,
            Difficulty = dto.Difficulty,
            Tags = dto.Tags ?? string.Empty
        };

        await _repository.AddAsync(challenge);
    }

    public async Task<ChallengeDisplayDto?> GetChallengeByIdAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true);
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
        
        var challenges = _repository.GetAll(includeDeleted: true);

        return await challenges.Select(c => new ChallengeDisplayDto(
                c.Id,
                c.Title,
                c.Description,
                c.Difficulty.ToString(),
                c.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray(),
                c.Submissions.Count
        )).ToListAsync();
    }

    public async Task UpdateChallengeAsync(EditChallengeDto editDto)
    {
        var challenge = await _repository.GetByIdAsync(editDto.Id, includeDeleted: true);
        if (challenge is null)
        {
            return;
        }

        challenge.Title = editDto.Title;
        challenge.Description = editDto.Description;
        challenge.Difficulty = editDto.Difficulty;
        challenge.Tags = editDto.Tags;

        await _repository.UpdateAsync(challenge);
    }
}
