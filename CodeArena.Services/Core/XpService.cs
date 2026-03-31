using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Services.Core;

public class XpService : IXpService
{
    private const int Multiplier = 50;
    private readonly IXpTransactionRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    public XpService(
        IXpTransactionRepository repository,
        UserManager<ApplicationUser> userManager
    )
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<ServiceResult<bool>> AwardXPAsync(ClaimsPrincipal user, int challengeId, Difficulty difficulty)
    {
        var userId = _userManager.GetUserId(user)!;  

        bool alreadyAwarded = await _repository
            .AnyAsync(x => x.UserId == userId && x.ChallengeId == challengeId);

        if (alreadyAwarded)
        {
            return ServiceResult<bool>.Fail(string.Format(XpAlreadyAwardedMessage, userId, challengeId));
        }

        int amount = difficulty switch
        {
            Difficulty.Easy => Multiplier,
            Difficulty.Medium => 2 * Multiplier,
            Difficulty.Hard => 3 * Multiplier,
            _ => default
        };

        var transaction = new XpTransaction
        {
            UserId = userId,
            ChallengeId = challengeId,
            XpAmount = amount,
            Reason = XpReason.ChallengeSolved
        };

        await _repository.AddAsync(transaction);

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<int>> GetTotalXPAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user)!;

        int totalXp = await _repository.GetAll()
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.XpAmount);    

        return ServiceResult<int>.Ok(totalXp);
    }
}
