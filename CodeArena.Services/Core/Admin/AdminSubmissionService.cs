using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Submission;
using CodeArena.Data.Common.Enums;
using static CodeArena.Common.ApplicationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Services.Core.Admin;

public class AdminSubmissionService : IAdminSubmissionService
{
    private readonly ISubmissionRepository _repository;

    public AdminSubmissionService(ISubmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SubmissionDisplayDto>> GetPendingSubmissionsAsync()
    {
        var submissions = _repository.GetAll()
            .Where(s => s.Status == SubmissionStatus.Pending);

        return await submissions.Select(s => new SubmissionDisplayDto
        (
            s.Id,            
            s.User.DisplayName,
            s.Challenge.Title,
            s.Language.ToString(),
            s.SubmittedAt
        )).ToListAsync();
    }

    public async Task<AdminSubmissionReviewDto?> GetSubmissionForReviewAsync(int id)
    {
        var submission = await _repository.GetByIdAsync(id);
        if (submission is null)
        {
            return null;
        }

        return new AdminSubmissionReviewDto
        (
            submission.Id,            
            submission.Challenge.Title,
            submission.User.DisplayName,
            submission.Language.ToString(),
            submission.Challenge.Difficulty.ToString(),
            submission.SolutionCode,
            submission.SubmittedAt
        );

    }
}
