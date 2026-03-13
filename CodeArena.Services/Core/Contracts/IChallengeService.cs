using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface IChallengeService
{
    Task<(IEnumerable<ChallengeDisplayDto>, int count)> GetChallengesAsync(
        int page = 1,
        int pageSize = 10,
        ChallengeStatus? statusFilter = ChallengeStatus.All,
        Difficulty? difficultyFilter = null,
        ClaimsPrincipal? user = null
    );

    Task<ChallengeDisplayDto?> GetChallengeByIdAsync(int id, ClaimsPrincipal? user = null);
}
