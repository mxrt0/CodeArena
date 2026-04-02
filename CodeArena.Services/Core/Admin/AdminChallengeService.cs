using CodeArena.Common;
using CodeArena.Common.Enums;
using CodeArena.Common.Exceptions;
using CodeArena.Common.Utilities;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.Extensions;
using CodeArena.Services.Helpers;
using CodeArena.Services.QueryModels.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Common.ApplicationConstants;

namespace CodeArena.Services.Core.Admin;

public class AdminChallengeService : IAdminChallengeService
{
    private readonly IChallengeRepository _repository;
    private readonly IMemoryCache _cache;

    public AdminChallengeService(
        IChallengeRepository repository,
        IMemoryCache cache
    )
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task CreateChallengeAsync(CreateChallengeDto dto)
    {
        var slugSet = await _repository.GetExistingSlugsAsync();

        var challenge = new Challenge
        {
            Title = dto.Title,
            Description = dto.Description,
            Difficulty = dto.Difficulty,
            Tags = dto.Tags is not null 
            ? string.Join(",",
                dto.Tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLowerInvariant())) 
            : string.Empty,
            Slug = SlugGenerator.GenerateUnique(dto.Title, slugSet)
        };

        await _repository.AddAsync(challenge);
    }

    public async Task DeleteChallengeAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true) ?? throw new ChallengeNotFoundException(id);

        if (challenge.IsDeleted) throw new ChallengeAlreadyDeletedException(id);

        await _repository.DeleteAsync(challenge);

        CacheHelper.Remove(
            _cache,
            string.Format(CacheKey_ChallengeBySlug, challenge.Slug)
        );
    }

    public async Task<ChallengeDisplayDto> GetChallengeByIdAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new ChallengeNotFoundException(id);

        return ChallengeMapper.ToDto(challenge);
    }


    public async Task<(IEnumerable<ChallengeDisplayDto>, int count)> GetChallengesAsync(
        AdminChallengeQuery query)
    {
        var challenges = _repository.GetAll(includeDeleted: true)
                                     .ApplyFiltering(query);

        var totalCount = await challenges.CountAsync();

        var dtos = await challenges
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => ChallengeMapper.ToDto(c))
            .ToListAsync();

        return (dtos, totalCount);
    }

    public async Task RestoreChallengeAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new ChallengeNotFoundException(id);

        if (!challenge.IsDeleted) throw new ChallengeAlreadyActiveException(id);

        await _repository.RestoreAsync(challenge);

        CacheHelper.Remove(
            _cache,
            string.Format(CacheKey_ChallengeBySlug, challenge.Slug)
        );
    }

    public async Task UpdateChallengeAsync(EditChallengeDto editDto)
    {
        var challenge = await _repository.GetByIdAsync(editDto.Id, includeDeleted: true)
            ?? throw new ChallengeNotFoundException(editDto.Id);

        challenge.Title = editDto.Title;
        challenge.Description = editDto.Description;
        challenge.Difficulty = editDto.Difficulty;
        challenge.Tags = editDto.Tags;

        await _repository.UpdateAsync(challenge);

        CacheHelper.Remove(
            _cache,
            string.Format(CacheKey_ChallengeBySlug, challenge.Slug)
        );
    }
}
