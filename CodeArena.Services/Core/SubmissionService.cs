using CodeArena.Common;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Submission;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CodeArena.Common.OutputMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodeArena.Services.Results;

namespace CodeArena.Services.Core;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubmissionService(
        ISubmissionRepository repository,
        UserManager<ApplicationUser> userManager
        )
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task CancelPendingAsync(int challengeId, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);

        var submission = await _repository.FirstOrDefaultAsync(s =>
            s.ChallengeId == challengeId &&
            s.UserId == userId &&
            s.Status == SubmissionStatus.Pending);

        if (submission is null)
        {
            return;
        }

        await _repository.RemoveAsync(submission);
    }

    public async Task CreateSubmissionAsync(SubmissionCreateDto createDto, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);

        if (userId is null)
        {
            throw new InvalidOperationException(UnauthenticatedUserSubmissionAttemptMessage);
        }
        if (await HasPendingSubmissionAsync(createDto.ChallengeId, user))
        {
            throw new InvalidOperationException(UserAlreadyHasPendingSubmissionMessage);
        }

        var submission = new Submission
        {
            ChallengeId = createDto.ChallengeId,
            SolutionCode = createDto.SolutionCode,
            Language = createDto.Language,
            UserId = userId,
            Status = SubmissionStatus.Pending
        };
        await _repository.AddAsync(submission);
    }

    public async Task<ServiceResult<SubmissionDetailsDto>> GetSubmissionDetailsAsync(int id, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (userId is null)
        {
            return ServiceResult<SubmissionDetailsDto>.Fail(UnauthenticatedActionMessage);
        }
        if (!await _repository.AnyAsync(s => s.Id == id))
        {
            return ServiceResult<SubmissionDetailsDto>.Fail(string.Format(SubmissionNotFoundMessage, id));
        }
        if (!await _repository.AnyAsync(s => s.Id == id && s.UserId == userId))
        {
            return ServiceResult<SubmissionDetailsDto>.Fail(string.Format(UnauthorizedActionMessage, userId));
        }

        var dto = await _repository.GetAll()
            .Where(s => s.Id == id && s.UserId == userId)
            .Include(s => s.Challenge)
            .Select(s => new SubmissionDetailsDto(
                s.Id,
                s.Challenge.Slug,
                s.Challenge.Title,
                s.Language.ToString(),
                s.Status.ToString(),
                s.Feedback ?? NoFeedbackMessage,
                s.SolutionCode,
                s.SubmittedAt
            ))
            .FirstAsync();

        return ServiceResult<SubmissionDetailsDto>.Ok(dto);
    }

    public async Task<(IEnumerable<SubmissionDisplayDto>, int count)> GetUserSubmissionsAsync(
        ClaimsPrincipal user,
        int page = 1,
        int pageSize = 10
    )
    {
        var userId = _userManager.GetUserId(user);
        var submissions = _repository.GetAll()
                              .Where(s => s.UserId == userId)
                              .Include(s => s.Challenge);

        var totalCount = await submissions.CountAsync();

        var dtos = await submissions
            .OrderByDescending(s => s.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SubmissionDisplayDto
            (
                s.Id,
                s.Challenge.Slug,
                s.Challenge.Title,
                s.Language.ToString(),
                s.Status.ToString(),
                string.IsNullOrWhiteSpace(s.Feedback)
                    ? NoFeedbackMessage
                    : s.Feedback,
                s.SubmittedAt
            ))
            .ToListAsync();

        return (dtos, totalCount);
    }

    public async Task<bool> HasApprovedSubmissionAsync(int challengeId, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        return await _repository.AnyAsync(s =>
            s.ChallengeId == challengeId &&
            s.UserId == userId &&
            s.Status == SubmissionStatus.Approved);
    }

    public async Task<bool> HasPendingSubmissionAsync(int challengeId, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        return await _repository.AnyAsync(s => 
            s.ChallengeId == challengeId &&
            s.UserId == userId &&
            s.Status == SubmissionStatus.Pending);
    }
}
