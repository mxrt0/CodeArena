using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.Challenge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Controllers;

public class ChallengesController : BaseController
{
    private readonly IChallengeService _service;

    public ChallengesController(IChallengeService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var challenges = await _service.GetChallengesAsync();
        var vm = new ChallengeIndexViewModel
        {
           Challenges = challenges
        };
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var challenge = await _service.GetChallengeByIdAsync(id);
        if (challenge is null)
        {
            return NotFound();
        }
        var vm = new ChallengeDetailsViewModel
        {
            Challenge = challenge
        };
        return View(vm);
    }
}
