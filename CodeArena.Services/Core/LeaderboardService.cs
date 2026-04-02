using CodeArena.Data.Common.Enums;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Leaderboard;
using CodeArena.Services.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static CodeArena.Common.ApplicationConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core;

public class LeaderboardService : ILeaderboardService
{
    private readonly IXpTransactionRepository _repository;
    private readonly IMemoryCache _cache;

    public LeaderboardService(
        IXpTransactionRepository repository,
        IMemoryCache cache
    )
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ServiceResult<List<LeaderboardEntryDto>>> GetLeaderboardAsync(int topN = 10)
    {
        if (_cache.TryGetValue(
            CacheKey_Leaderboard,
            out List<LeaderboardEntryDto>? cachedList))
        {
            return ServiceResult<List<LeaderboardEntryDto>>.Ok(cachedList!);
        }
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

        _cache.Set(
            CacheKey_Leaderboard,
            leaderboard,
            TimeSpan.FromMinutes(CacheDuration_Leaderboard_Minutes));

        return ServiceResult<List<LeaderboardEntryDto>>.Ok(leaderboard);
    }
}
