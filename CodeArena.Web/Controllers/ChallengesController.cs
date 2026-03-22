using CodeArena.Common.Enums;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.Challenge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CodeArena.Web.Controllers;

public class ChallengesController : BaseController
{
    const int PageSize = 12;
    private readonly IChallengeService _service;
    private readonly ISubmissionService _submissionService;
    private readonly ILogger<ChallengesController> _logger;

    public ChallengesController(
        IChallengeService service,
        ISubmissionService submissionService,
        ILogger<ChallengesController> logger
    )
    {
        _service = service;
        _submissionService = submissionService;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(ChallengeIndexViewModel inputVm, int page = 1)
    {
        var (challenges, count) = await _service.GetChallengesAsync(
            page: Math.Max(1, page),
            pageSize: PageSize,
            statusFilter: inputVm.StatusFilter,
            difficultyFilter: inputVm.SelectedDifficulty,
            user: User?.Identity?.IsAuthenticated ?? false
                    ? User
                    : null);

        var vm = new ChallengeIndexViewModel
        {
           Challenges = challenges,
           SelectedDifficulty = inputVm.SelectedDifficulty,
           StatusFilter = inputVm.StatusFilter,
           CurrentPage = page,
           TotalPages = (int)Math.Ceiling(count / (double)PageSize)
        };
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var result = await _service.GetChallengeByIdAsync(
            id,
          user: User?.Identity?.IsAuthenticated ?? false
                    ? User
                    : null);

        if (!result.Success)
        {
            _logger.LogInformation(result.ErrorMessage);
            return NotFound();
        }
        var vm = new ChallengeDetailsViewModel
        {
            Challenge = result.Data!
        };
        
        return View(vm);
    }
}
