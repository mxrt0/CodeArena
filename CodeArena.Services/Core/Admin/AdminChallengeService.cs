using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
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
        };

        await _repository.AddAsync(challenge);
    }
}
