using CodeArena.Services.DTOs.Leaderboard;
using CodeArena.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Contracts;

public interface ILeaderboardService
{
    Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync(int topN = 10);
}
