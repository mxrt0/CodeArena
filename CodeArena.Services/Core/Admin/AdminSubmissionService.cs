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

    public async Task<IEnumerable<AdminSubmissionReviewDto>> GetPendingSubmissionsAsync()
    {
        var submissions = _repository.GetAll()
            .Where(s => s.Status == SubmissionStatus.Pending)
            .Include(s => s.Challenge)
            .Include(s => s.User);

        return await submissions.Select(s => new AdminSubmissionReviewDto
        (
            s.Id,
            s.Challenge.Title,
            s.User.DisplayName,
            s.Language.ToString(),
            s.Challenge.Difficulty.ToString(),
            s.SolutionCode,
            s.SubmittedAt.ToString(DefaultDateFormat)
        )).ToListAsync();
    }
}
