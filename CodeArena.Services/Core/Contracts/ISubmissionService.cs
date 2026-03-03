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
    bool HasPendingSubmission(int challengeId, ClaimsPrincipal user);
}
