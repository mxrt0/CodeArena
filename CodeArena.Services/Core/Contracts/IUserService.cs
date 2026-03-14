using CodeArena.Services.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface IUserService
{
    Task<UserStatsDto> GetUserStatsAsync(ClaimsPrincipal user); 
}
