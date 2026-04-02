using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodeArena.Services.Results;
using CodeArena.Services.QueryModels;

namespace CodeArena.Services.Core.Contracts;

public interface IChallengeService
{
    Task<(IEnumerable<ChallengeDisplayDto>, int count)> GetChallengesAsync(
        ChallengeQuery query,
        ClaimsPrincipal? user = null
    );
    Task<IEnumerable<string>> GetAllTagsAsync();
    Task<ServiceResult<ChallengeDisplayDto>> GetChallengeByIdAsync(int id, ClaimsPrincipal? user = null);
    Task<ServiceResult<ChallengeDisplayDto>> GetChallengeBySlugAsync(string slug, ClaimsPrincipal? user = null);
}
