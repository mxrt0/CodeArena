using CodeArena.Services.DTOs.Submission;
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
    Task CancelPendingAsync(int challengeId, ClaimsPrincipal user);
    Task<IEnumerable<SubmissionDisplayDto>> GetUserSubmissionsAsync(ClaimsPrincipal user);
}
