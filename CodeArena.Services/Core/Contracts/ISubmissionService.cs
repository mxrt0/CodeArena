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
    Task CreateSubmissionAsync(SubmissionCreateDto createDto, ClaimsPrincipal user);
    Task<bool> HasPendingSubmissionAsync(int challengeId, ClaimsPrincipal user);
    Task<bool> HasApprovedSubmissionAsync(int challengeId, ClaimsPrincipal user);
    Task CancelPendingAsync(int challengeId, ClaimsPrincipal user);
    Task<(IEnumerable<SubmissionDisplayDto>, int count)> GetUserSubmissionsAsync(   
        SubmissionQuery query,
        ClaimsPrincipal user
    );
    Task<ServiceResult<SubmissionDetailsDto>> GetSubmissionDetailsAsync(int id, ClaimsPrincipal user);
}
