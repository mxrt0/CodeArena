using CodeArena.Services.DTOs.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface IChallengeService
{
    Task<IEnumerable<ChallengeDisplayDto>> GetChallengesAsync();
    Task<ChallengeDisplayDto?> GetChallengeByIdAsync(int id);
}
