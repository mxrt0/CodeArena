using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Submission;
using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeArena.Common.Exceptions;
using static CodeArena.Common.OutputMessages;
using static CodeArena.Common.ApplicationConstants;
using Microsoft.Extensions.Caching.Memory;
using CodeArena.Services.QueryModels;
using CodeArena.Services.Extensions;
using CodeArena.Services.Core.Contracts;

namespace CodeArena.Services.Core.Admin;

public class AdminSubmissionService : IAdminSubmissionService
{
    private readonly ISubmissionRepository _repository;
    private readonly IXpService _xpService;
    private readonly IMemoryCache _cache;

    public AdminSubmissionService(
        ISubmissionRepository repository,
        IMemoryCache cache,
        IXpService xpService
    )
    {
        _repository = repository;
        _cache = cache;
        _xpService = xpService;
    }

    public async Task ApproveAsync(int id, string? feedback = null)
    {
        feedback ??= NoFeedbackMessage;

        var submission = await _repository.GetByIdAsync(id) ?? throw new SubmissionNotFoundException(id);
      
        if (submission.Status == SubmissionStatus.Approved) 
            throw new SubmissionAlreadyApprovedException(id);

        if (submission.Status == SubmissionStatus.Rejected)
            throw new SubmissionAlreadyRejectedException(id);

        submission.Status = SubmissionStatus.Approved;
        submission.Feedback = feedback;

        await _repository.UpdateAsync(submission);

        await _xpService.AwardXpAsync(submission.UserId, submission.ChallengeId,
            submission.Challenge.Difficulty);

        InvalidateCache(
            CacheKey_Leaderboard,
            CacheKey_PendingSubmissions,
            CacheKey_SubmissionsAll,
            string.Format(CacheKey_Admin_SubmissionById, id),
            string.Format(CacheKey_User_SubmissionById, id),
            string.Format(CacheKey_UserStats_ByUserId, submission.UserId)
        );
    }

    public async Task<(IEnumerable<SubmissionDisplayDto>, int count)> GetPendingSubmissionsAsync(
       int page = 1,
       int pageSize = 10,
       SubmissionLanguage? languageFilter = null
    )
    {
        var query = new SubmissionQuery
        {
            Language = languageFilter
        };

        var submissions = _repository.GetAll()
            .Where(s => s.Status == SubmissionStatus.Pending)
            .ApplyFiltering(query);
        
        var totalCount = await submissions.CountAsync();

        var dtos = await submissions
        .OrderByDescending(s => s.SubmittedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(s => new SubmissionDisplayDto
        (
            s.Id,            
            s.User.DisplayName,
            s.Challenge.Title,
            s.Language.ToString(),
            s.SubmittedAt
        ))
        .ToListAsync();

        return (dtos, totalCount);
    }

    public async Task<AdminSubmissionReviewDto> GetSubmissionForReviewAsync(int id)
    {
        if (_cache.TryGetValue(
            string.Format(CacheKey_Admin_SubmissionById, id),
            out AdminSubmissionReviewDto? cachedDto))
        {
            return cachedDto!;
        }
        var submission = await _repository.GetByIdAsync(id) ?? throw new SubmissionNotFoundException(id);

        var dto = new AdminSubmissionReviewDto
        (
            submission.Id,            
            submission.Challenge.Title,
            submission.User.DisplayName,
            submission.Language.ToString(),
            submission.Challenge.Difficulty.ToString(),
            submission.SolutionCode,
            submission.SubmittedAt
        );

        _cache.Set(
            string.Format(CacheKey_Admin_SubmissionById, id),
            dto,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(CacheDuration_SubmissionById_Minutes)
            }
        );

        return dto;
    }

    public async Task RejectAsync(int id, string? feedback = null)
    {
        feedback ??= NoFeedbackMessage;

        var submission = await _repository.GetByIdAsync(id) ?? throw new SubmissionNotFoundException(id);

        if (submission.Status == SubmissionStatus.Approved)
            throw new SubmissionAlreadyApprovedException(id);

        if (submission.Status == SubmissionStatus.Rejected)
            throw new SubmissionAlreadyRejectedException(id);

        submission.Status = SubmissionStatus.Rejected;
        submission.Feedback = feedback;

        await _repository.UpdateAsync(submission);

        InvalidateCache(
            CacheKey_PendingSubmissions,
            CacheKey_SubmissionsAll,
            string.Format(CacheKey_Admin_SubmissionById, id),
            string.Format(CacheKey_User_SubmissionById, id),
            string.Format(CacheKey_UserStats_ByUserId, submission.UserId)
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
