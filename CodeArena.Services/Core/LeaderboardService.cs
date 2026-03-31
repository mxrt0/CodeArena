using CodeArena.Data.Common.Enums;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Leaderboard;
using CodeArena.Services.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core;

public class LeaderboardService : ILeaderboardService
{
    private readonly IXpTransactionRepository _repository;

    public LeaderboardService(IXpTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync(int topN = 10)
    {
        var leaderboard = await _repository.GetAll()
            .GroupBy(x => new { x.UserId, x.User.DisplayName })
            .Select(g => new LeaderboardEntryDto
            {
                UserId = g.Key.UserId,
                DisplayName = g.Key.DisplayName,
                TotalXp = g.Sum(x => x.XpAmount),
                ChallengesSolved = g.Count(x => x.Reason == XpReason.ChallengeSolved)
            })
            .OrderByDescending(x => x.TotalXp)
            .ThenBy(x => x.DisplayName)
            .Take(topN)
            .ToListAsync();

        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }

        return ServiceResult<List<LeaderboardEntryDto>>.Ok(leaderboard);
    }
}
