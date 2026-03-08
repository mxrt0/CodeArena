using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Web.Models.Challenge;
using CodeArena.Web.Models.Submission;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Controllers;

public class SubmissionsController : BaseController
{
    private readonly ISubmissionService _submissionService;
    private readonly IChallengeService _challengeService;

    public SubmissionsController(
        ISubmissionService submissionService,
        IChallengeService challengeService)
    {
        _submissionService = submissionService;
        _challengeService = challengeService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new SubmissionsIndexViewModel
        {
            Submissions = await _submissionService.GetUserSubmissionsAsync(User)
        };
        return View(vm);
    }

    public async Task<IActionResult> Create(SubmissionCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var challenge = await _challengeService.GetChallengeByIdAsync(createDto.ChallengeId);
            if (challenge is null)
            {
                return NotFound();
            }

            var vm = new ChallengeDetailsViewModel
            {
                Challenge = challenge,
                SolutionCode = createDto.SolutionCode,
                Language = createDto.Language
            };

            return View("~/Views/Challenges/Details.cshtml", vm);
        }
        await _submissionService.CreateSubmissionAsync(createDto, User);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int challengeId, bool redirectToSubmissions)
    {
        await _submissionService.CancelPendingAsync(challengeId, User);
        return redirectToSubmissions 
            ? RedirectToAction(nameof(Index))
            : RedirectToAction("Details", "Challenges", new { id = challengeId });
    }
}
