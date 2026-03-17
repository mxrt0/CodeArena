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

namespace CodeArena.Services.Core.Admin;

public class AdminSubmissionService : IAdminSubmissionService
{
    private readonly ISubmissionRepository _repository;

    public AdminSubmissionService(ISubmissionRepository repository)
    {
        _repository = repository;
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
    }

    public async Task<(IEnumerable<SubmissionDisplayDto>, int count)> GetPendingSubmissionsAsync(
       int page = 1,
       int pageSize = 10
    )
    {
        var submissions = _repository.GetAll()
            .Where(s => s.Status == SubmissionStatus.Pending);

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
        var submission = await _repository.GetByIdAsync(id) ?? throw new SubmissionNotFoundException(id);

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
    }
}
