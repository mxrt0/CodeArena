using CodeArena.Common.Enums;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.QueryModels;
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
    public async Task<IActionResult> Index(ChallengeQuery query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = PageSize;

        var (challenges, count) = await _service.GetChallengesAsync(
            query,
            user: User?.Identity?.IsAuthenticated ?? false
                    ? User
                    : null);

        var vm = new ChallengeIndexViewModel
        {
           Challenges = challenges,
           SelectedDifficulty = query.Difficulty,
           StatusFilter = query.Status,
           CurrentPage = query.Page,
           TotalPages = (int)Math.Ceiling(count / (double)PageSize),
           Tags = query.Tags,
           AvailableTags = await _service.GetAllTagsAsync(),
           Search = query.Search,
        };
        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest();
        }

        var result = await _service.GetChallengeBySlugAsync(
            slug,
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
