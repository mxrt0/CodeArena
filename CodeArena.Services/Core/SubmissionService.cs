using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Submission;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            throw new InvalidOperationException("User must be authenticated to create a submission.");
        }
        if (await HasPendingSubmissionAsync(createDto.ChallengeId, user))
        {
            throw new InvalidOperationException("User already has a pending submission for this challenge.");
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

    public async Task<bool> HasPendingSubmissionAsync(int challengeId, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        return await _repository.AnyAsync(s => 
            s.ChallengeId == challengeId &&
            s.UserId == userId &&
            s.Status == SubmissionStatus.Pending);
    }
}
