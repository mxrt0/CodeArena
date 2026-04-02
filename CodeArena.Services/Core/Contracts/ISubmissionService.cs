using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Services.QueryModels;
using CodeArena.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface ISubmissionService
{
    Task CreateSubmissionAsync(SubmissionCreateDto createDto, string userId);
    Task<bool> HasPendingSubmissionAsync(int challengeId, string userId);
    Task<bool> HasApprovedSubmissionAsync(int challengeId, string userId);
    Task CancelPendingAsync(int challengeId, string userId);
    Task<PagedResult<SubmissionDisplayDto>> GetUserSubmissionsAsync(   
        SubmissionQuery query,
        string userId
    );
    Task<ServiceResult<SubmissionDetailsDto>> GetSubmissionDetailsAsync(int id, string userId);
}
