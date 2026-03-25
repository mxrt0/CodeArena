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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core;

public class ChallengeService : IChallengeService
{
    private readonly IChallengeRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISubmissionService _submissionService;

    public ChallengeService(
        IChallengeRepository repository,
        UserManager<ApplicationUser> userManager,
        ISubmissionService submissionService
    )
    {
        _repository = repository;
        _userManager = userManager;
        _submissionService = submissionService;
    }

    public async Task<ServiceResult<ChallengeDisplayDto>> GetChallengeByIdAsync(int id, ClaimsPrincipal? user = null)
    {
        var challenge = await _repository.GetByIdAsync(id);
        if (challenge is null)
        {
            return ServiceResult<ChallengeDisplayDto>.Fail(string.Format(ChallengeNotFoundMessage, id));
        }

        var dto = new ChallengeDisplayDto(
            challenge.Id,
            challenge.Slug,
            challenge.Title,
            challenge.Description,
            challenge.Difficulty.ToString(),
            challenge.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray(),
            challenge.Submissions.Count,
            IsDeleted: false
        );

        dto.HasPendingSubmission = user is not null
                                    ? await _submissionService.HasPendingSubmissionAsync(dto.Id, user)
                                    : false;

        dto.IsSolved = user is not null 
                        ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, user) 
                        : false;

        return ServiceResult<ChallengeDisplayDto>.Ok(dto);
    }

    public async Task<ServiceResult<ChallengeDisplayDto>> GetChallengeBySlugAsync(string slug, ClaimsPrincipal? user = null)
    {
        var challenge = await _repository.GetBySlugAsync(slug);
        if (challenge is null)
        {
            return ServiceResult<ChallengeDisplayDto>.Fail(string.Format(ChallengeNotFoundMessage, slug));
        }

        var dto = new ChallengeDisplayDto(
            challenge.Id,
            challenge.Slug,
            challenge.Title,
            challenge.Description,
            challenge.Difficulty.ToString(),
            challenge.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray(),
            challenge.Submissions.Count,
            IsDeleted: false
        );

        dto.HasPendingSubmission = user is not null
                                    ? await _submissionService.HasPendingSubmissionAsync(dto.Id, user)
                                    : false;

        dto.IsSolved = user is not null
                        ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, user)
                        : false;

        return ServiceResult<ChallengeDisplayDto>.Ok(dto);
    }

    public async Task<(IEnumerable<ChallengeDisplayDto>, int count)> GetChallengesAsync(
        int page = 1,
        int pageSize = 10,
        ChallengeStatus? statusFilter = ChallengeStatus.All,
        Difficulty? difficultyFilter = null,
        ClaimsPrincipal? user = null
    )
    {
        var challenges = _repository.GetAll();
        if (statusFilter != ChallengeStatus.All && user is not null)
        {
            challenges = statusFilter switch
            {
                ChallengeStatus.Solved => challenges
                                          .Include(c => c.Submissions)
                                          .Where(c => c.Submissions.Any(s => s.Status == SubmissionStatus.Approved
                                                    && s.UserId == _userManager.GetUserId(user))),
               ChallengeStatus.Unsolved => challenges
                                          .Include(c => c.Submissions)
                                          .Where(c => !c.Submissions.Any(s => s.Status == SubmissionStatus.Approved
                                                    && s.UserId == _userManager.GetUserId(user))),
               _ => challenges
            };
        }

        if (difficultyFilter is not null && user is not null)
        {
            challenges = challenges.Where(c => c.Difficulty == difficultyFilter);
        }

        var totalCount = await challenges.CountAsync();

        var dtos =  await challenges
            .OrderBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ChallengeDisplayDto(
                c.Id,
                c.Slug,
                c.Title,
                c.Description,
                c.Difficulty.ToString(),
                c.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray(),
                c.Submissions.Count,
                false
        )).ToListAsync();

        foreach (var dto in dtos)
        {
            dto.HasPendingSubmission = user is not null
                                        ? await _submissionService.HasPendingSubmissionAsync(dto.Id, user)
                                        : false;

            dto.IsSolved = user is not null
                            ? await _submissionService.HasApprovedSubmissionAsync(dto.Id, user)
                            : false;
        }

        return (dtos, totalCount);
    }
}
