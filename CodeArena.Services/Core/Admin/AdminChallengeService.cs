using CodeArena.Common;
using CodeArena.Common.Exceptions;
using CodeArena.Common.Utilities;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.DTOs.Challenge;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static CodeArena.Common.ApplicationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        InvalidateCache(CacheKey_ChallengesAll);
    }

    public async Task DeleteChallengeAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true) ?? throw new ChallengeNotFoundException(id);

        if (challenge.IsDeleted) throw new ChallengeAlreadyDeletedException(id);

        await _repository.DeleteAsync(challenge);

        InvalidateCache(
            CacheKey_ChallengesAll,
            string.Format(CacheKey_ChallengeBySlug, challenge.Slug)
        );
    }

    public async Task<ChallengeDisplayDto> GetChallengeByIdAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new ChallengeNotFoundException(id);

        return new ChallengeDisplayDto(
            challenge.Id,
            challenge.Slug,
            challenge.Title,
            challenge.Description,
            challenge.Difficulty.ToString(),
            challenge.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToArray(),
            challenge.Submissions.Count,
            challenge.IsDeleted
        );
    }


    public async Task<IEnumerable<ChallengeDisplayDto>> GetChallengesAsync()
    {
        if (_cache.TryGetValue(CacheKey_ChallengesAll,
            out List<ChallengeDisplayDto>? cachedList))
        {
            return cachedList!;
        }
        var challenges = _repository.GetAll(includeDeleted: true);

        var dtos = await challenges.Select(c => new ChallengeDisplayDto(
                c.Id,
                c.Slug,
                c.Title,
                c.Description,
                c.Difficulty.ToString(),
                c.Tags
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToArray(),
                c.Submissions.Count,
                c.IsDeleted
        )).ToListAsync();

        _cache.Set(
            CacheKey_ChallengesAll,
            dtos,
            TimeSpan.FromMinutes(CacheDuration_ChallengesAll_Minutes)
        );

        return dtos;
    }

    public async Task RestoreChallengeAsync(int id)
    {
        var challenge = await _repository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new ChallengeNotFoundException(id);

        if (!challenge.IsDeleted) throw new ChallengeAlreadyActiveException(id);

        await _repository.RestoreAsync(challenge);

        InvalidateCache(
            CacheKey_ChallengesAll,
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

        InvalidateCache(
            CacheKey_ChallengesAll,
            string.Format(CacheKey_ChallengeBySlug, challenge.Slug)
        );
    }
    private void InvalidateCache(params string[] keys)
    {
        if (!keys.Any()) return;
        foreach (var key in keys)
        {
            _cache.Remove(key);
        }
    }
}
