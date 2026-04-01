using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.User;
using Microsoft.AspNetCore.Identity;
using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static CodeArena.Common.ApplicationConstants;
using CodeArena.Common.Utilities;

namespace CodeArena.Services.Core;

public class UserService : IUserService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemoryCache _cache;
    private readonly IXpService _xpService;

    public UserService(
        ISubmissionRepository repository,
        UserManager<ApplicationUser> userManager,
        IMemoryCache cache,
        IXpService xpService)
    {
        _submissionRepository = repository;
        _userManager = userManager;
        _cache = cache;
        _xpService = xpService;
    }

    public async Task<UserStatsDto> GetUserStatsAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user)!;

        if (_cache.TryGetValue(
            string.Format(CacheKey_UserStats_ByUserId, userId),
            out UserStatsDto? cachedStats))
        {
            return cachedStats!;
        }

        var submissions = _submissionRepository.GetAll()
            .Where(s => s.UserId == userId);

        var solved = await submissions
            .Where(s => s.Status == SubmissionStatus.Approved)
            .Include(s => s.Challenge)
            .ToListAsync();

        var totalXp = (await _xpService.GetTotalXpAsync(userId)).Data!;
        var level = LevelCalculator.CalculateLevel(totalXp);
        var (currentXp, neededXp) = LevelCalculator.GetProgress(totalXp);

        var dto = new UserStatsDto(
            solved.Count,
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Easy),
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Medium),
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Hard),
            await submissions.CountAsync(s => s.Status == SubmissionStatus.Pending),
            await submissions.CountAsync(s => s.Status == SubmissionStatus.Rejected),
            totalXp,
            level,
            currentXp,
            neededXp
        );

        _cache.Set(
            string.Format(CacheKey_UserStats_ByUserId, userId),
            dto,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(CacheDuration_UserStats_Minutes)
            }
        );

        return dto;
    }
}
