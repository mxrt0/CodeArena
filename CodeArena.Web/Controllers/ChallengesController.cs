using CodeArena.Common.Enums;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.Challenge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CodeArena.Web.Controllers;

public class ChallengesController : BaseController
{
    private readonly IChallengeService _service;
    private readonly ISubmissionService _submissionService;

    public ChallengesController(IChallengeService service, ISubmissionService submissionService)
    {
        _service = service;
        _submissionService = submissionService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(ChallengeIndexViewModel filter)
    {
        var challenges = await _service.GetChallengesAsync(
            statusFilter: filter.StatusFilter,
            user: User?.Identity?.IsAuthenticated ?? false
                    ? User
                    : null);

        if (!string.IsNullOrWhiteSpace(filter.SelectedDifficulty))
        {
            challenges = challenges.Where(c => c.Difficulty == filter.SelectedDifficulty);
        }

        var vm = new ChallengeIndexViewModel
        {
           Challenges = challenges,
           SelectedDifficulty = filter.SelectedDifficulty,
           StatusFilter = filter.StatusFilter,
        };
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var challenge = await _service.GetChallengeByIdAsync(
            id,
          user: User?.Identity?.IsAuthenticated ?? false
                    ? User
                    : null);

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
