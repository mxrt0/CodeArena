using CodeArena.Common.Enums;
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
    Task<IEnumerable<ChallengeDisplayDto>> GetChallengesAsync(
        ChallengeStatus? statusFilter = ChallengeStatus.All,
        ClaimsPrincipal? user = null
    );

    Task<ChallengeDisplayDto?> GetChallengeByIdAsync(int id, ClaimsPrincipal? user = null);
}
