using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Admin.Submission;
using CodeArena.Services.QueryModels;
using CodeArena.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin.Contracts;

public interface IAdminSubmissionService
{
    Task<PagedResult<SubmissionDisplayDto>> GetPendingSubmissionsAsync(SubmissionQuery query);
    Task<AdminSubmissionReviewDto> GetSubmissionForReviewAsync(int id);
    Task ApproveAsync(int id, string? feedback = null);
    Task RejectAsync(int id, string? feedback = null);
}
