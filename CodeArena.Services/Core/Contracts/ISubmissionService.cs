using CodeArena.Services.DTOs.Submission;
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
        ClaimsPrincipal user,
        int page = 1,
        int pageSize = 10
    );
    Task<ServiceResult<SubmissionDetailsDto>> GetSubmissionDetailsAsync(int id, ClaimsPrincipal user);
}
