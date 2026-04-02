using CodeArena.Common.Enums;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.QueryModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin.Contracts;

public interface IAdminChallengeService
{
    Task CreateChallengeAsync(CreateChallengeDto dto);
    Task<(IEnumerable<ChallengeDisplayDto>, int count)> GetChallengesAsync(AdminChallengeQuery query);
    Task<ChallengeDisplayDto> GetChallengeByIdAsync(int id);
    Task UpdateChallengeAsync(EditChallengeDto editDto);
    Task DeleteChallengeAsync(int id);
    Task RestoreChallengeAsync(int id); 
}
