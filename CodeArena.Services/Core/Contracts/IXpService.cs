using CodeArena.Data.Common.Enums;
using CodeArena.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface IXpService
{
    Task<ServiceResult<bool>> AwardXPAsync(ClaimsPrincipal user, int challengeId, Difficulty difficulty);
    Task<ServiceResult<int>> GetTotalXPAsync(ClaimsPrincipal user);
}
