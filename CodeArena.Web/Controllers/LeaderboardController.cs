using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.Leaderboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeArena.Web.Controllers;

public class LeaderboardController : BaseController
{
    const int LeaderboardSize = 10;
    private readonly ILeaderboardService _service;

    public LeaderboardController(ILeaderboardService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var leaderboard = await _service.GetLeaderboardAsync(LeaderboardSize);

        var vm = new LeaderboardIndexViewModel
        {
            Leaderboard = leaderboard.Data!,
            CurrentUserId = UserId ?? string.Empty
        };

        return View(vm);
    }
}
