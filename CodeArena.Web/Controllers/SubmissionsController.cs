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
    const int PageSize = 2;
    private readonly ISubmissionService _submissionService;
    private readonly IChallengeService _challengeService;

    public SubmissionsController(
        ISubmissionService submissionService,
        IChallengeService challengeService)
    {
        _submissionService = submissionService;
        _challengeService = challengeService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var (submissions, count) = await _submissionService.GetUserSubmissionsAsync(
                User,
                page,
                PageSize
            );
        var vm = new SubmissionsIndexViewModel
        {
            Submissions = submissions,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(count / (double)PageSize)
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

    public async Task<IActionResult> Details(int id)
    {
        var submission = await _submissionService.GetSubmissionDetailsAsync(id, User);
        if (submission is null)
        {
            return NotFound();
        }

        var vm = new SubmissionDetailsViewModel 
        { 
            Submission = submission
        };
        return View(vm);     
    }
}
