using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Admin.Submission;
using CodeArena.Services.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin.Contracts;

public interface IAdminSubmissionService
{
    Task<(IEnumerable<SubmissionDisplayDto>, int count)> GetPendingSubmissionsAsync(SubmissionQuery query);
    Task<AdminSubmissionReviewDto> GetSubmissionForReviewAsync(int id);
    Task ApproveAsync(int id, string? feedback = null);
    Task RejectAsync(int id, string? feedback = null);
}
