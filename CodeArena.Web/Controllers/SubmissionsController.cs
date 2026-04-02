using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Web.Models.Challenge;
using CodeArena.Web.Models.Submission;
using static CodeArena.Common.OutputMessages;
using static CodeArena.Common.ApplicationConstants;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeArena.Services.QueryModels;

namespace CodeArena.Web.Controllers;

public class SubmissionsController : BaseController
{
    const int PageSize = 10;
    private readonly ISubmissionService _submissionService;
    private readonly IChallengeService _challengeService;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        ISubmissionService submissionService,
        IChallengeService challengeService,
        ILogger<SubmissionsController> logger)
    {
        _submissionService = submissionService;
        _challengeService = challengeService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(SubmissionQuery query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = PageSize;

        var result = await _submissionService.GetUserSubmissionsAsync(query, User);

        var vm = new SubmissionsIndexViewModel
        {
            Submissions = result.Items,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)PageSize),
            Language = query.Language,
            Status = query.Status
        };
        return View(vm);
    }

    public async Task<IActionResult> Create(SubmissionCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var result = await _challengeService.GetChallengeByIdAsync(createDto.ChallengeId);
            if (!result.Success)
            {
                return NotFound();
            }

            var vm = new ChallengeDetailsViewModel
            {
                Challenge = result.Data!,
                SolutionCode = createDto.SolutionCode,
                Language = createDto.Language
            };

            return View("~/Views/Challenges/Details.cshtml", vm);
        }
        await _submissionService.CreateSubmissionAsync(createDto, User);
        TempData[SuccessTempDataKey] = SubmissionCreatedMessage;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int challengeId, bool redirectToSubmissions)
    {
        await _submissionService.CancelPendingAsync(challengeId, User);
        TempData[InfoTempDataKey] = SubmissionCancelledMessage; 
        return redirectToSubmissions 
            ? RedirectToAction(nameof(Index))
            : RedirectToAction("Details", "Challenges", new { id = challengeId });
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var result = await _submissionService.GetSubmissionDetailsAsync(id, User);
        if (!result.Success)
        {
            _logger.LogInformation(result.ErrorMessage);
            return NotFound();
        }

        var vm = new SubmissionDetailsViewModel 
        { 
            Submission = result.Data!
        };
        return View(vm);     
    }
}
