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
    public async Task<IActionResult> Index()
    {
        var challenges = await _service.GetChallengesAsync();
        foreach (var challenge in challenges)
        {
            challenge.IsSolved = await _submissionService.HasApprovedSubmissionAsync(challenge.Id, User);
        }
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
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            vm.HasPendingSubmission = await _submissionService.HasPendingSubmissionAsync(id, User);
            vm.HasApprovedSubmission = await _submissionService.HasApprovedSubmissionAsync(id, User);
        }
        return View(vm);
    }
}
