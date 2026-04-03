using CodeArena.Common;
using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.Results;
using Microsoft.AspNetCore.Identity;
using static CodeArena.Common.OutputMessages;
using static CodeArena.Common.ApplicationConstants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using CodeArena.Services.QueryModels;
using CodeArena.Services.Extensions;
using CodeArena.Services.Helpers;

namespace CodeArena.Services.Core;

public class ChallengeService : IChallengeService
{
    private readonly IChallengeRepository _repository;
    private readonly ISubmissionService _submissionService;
    private readonly IMemoryCache _cache;

    public ChallengeService(
        IChallengeRepository repository,
        ISubmissionService submissionService,
        IMemoryCache cache
    )
    {
        _repository = repository;
        _submissionService = submissionService;
        _cache = cache;
    }

    public async Task<IEnumerable<string>> GetAllTagsAsync()
    {
        var rawTags = await _repository.GetAll()
        .Select(c => c.Tags)
        .ToListAsync();

        return rawTags
            .SelectMany(tags => tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim()))
            .Distinct()
            .OrderBy(t => t)
            .ToList();
    }

    public async Task<ServiceResult<ChallengeDisplayDto>> GetChallengeByIdAsync(int id, string? userId)
    {
        var challenge = await _repository.GetByIdAsync(id);
        if (challenge is null)
        {
            return ServiceResult<ChallengeDisplayDto>.Fail(string.Format(ChallengeNotFoundMessage, id));
        }

        var dto = ChallengeMapper.ToDto(challenge);

        dto.HasPendingSubmission = userId is not null
                                    ? await _submissionService.HasPendingSubmissionAsync(dto.Id, userId)
                                    : false;

        dto.IsSolved = userId is not null 
                        ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, userId) 
                        : false;

        return ServiceResult<ChallengeDisplayDto>.Ok(dto);
    }

    public async Task<ServiceResult<ChallengeDisplayDto>> GetChallengeBySlugAsync(string slug,
        string? userId)
    {
        if (_cache.TryGetValue(
            string.Format(CacheKey_ChallengeBySlug, slug),
            out ChallengeDisplayDto? cachedDto))
        {
            return ServiceResult<ChallengeDisplayDto>.Ok(cachedDto!);
        }
        var challenge = await _repository.GetBySlugAsync(slug);
        if (challenge is null)
        {
            return ServiceResult<ChallengeDisplayDto>.Fail(string.Format(ChallengeNotFoundMessage, slug));
        }

        var dto = ChallengeMapper.ToDto(challenge);

        dto.HasPendingSubmission = userId is not null
                                    ? await _submissionService.HasPendingSubmissionAsync(dto.Id, userId)
                                    : false;

        dto.IsSolved = userId is not null
                        ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, userId)
                        : false;

        _cache.Set(
            string.Format(CacheKey_ChallengeBySlug, slug),
            dto,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(CacheDuration_ChallengeBySlug_Minutes)
            }
        );

        return ServiceResult<ChallengeDisplayDto>.Ok(dto);
    }

    public async Task<PagedResult<ChallengeDisplayDto>> GetChallengesAsync(
        ChallengeQuery query,
        string? userId
    )
    {
        var challenges = _repository.GetAll();

        if (query.Status is not ChallengeStatus.All && userId is not null)
        {
            challenges = query.Status switch
            {
                ChallengeStatus.Solved => challenges
                                          .Where(c => c.Submissions.Any(s => s.Status == SubmissionStatus.Approved
                                                    && s.UserId == userId)),
               ChallengeStatus.Unsolved => challenges
                                          .Where(c => !c.Submissions.Any(s => s.Status == SubmissionStatus.Approved
                                                    && s.UserId == userId)),
               _ => challenges
            };
        }
       
        challenges = challenges.ApplyFiltering(query);

        var totalCount = await challenges.CountAsync();

        var dtos = await challenges
            .OrderBy(c => c.Title)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => ChallengeMapper.ToDto(c))
            .ToListAsync();

        foreach (var dto in dtos)
        {
            dto.HasPendingSubmission = userId is not null
                                        ? await _submissionService.HasPendingSubmissionAsync(dto.Id, userId)
                                        : false;

            dto.IsSolved = userId is not null
                            ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, userId)
                            : false;
        }

        return new PagedResult<ChallengeDisplayDto>(dtos, totalCount);
    }

}
