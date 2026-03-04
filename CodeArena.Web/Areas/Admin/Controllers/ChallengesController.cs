using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class ChallengesController : BaseAdminController
{
    private readonly IAdminChallengeService _challengeService;

    public ChallengesController(IAdminChallengeService challengeService)
    {
        _challengeService = challengeService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateChallengeDto dto)
    {
        if (!ModelState.IsValid)
        {
            var vm = new CreateChallengeViewModel
            {
                Challenge = dto
            };
            return View(vm);
        }
        await _challengeService.CreateChallengeAsync(dto);
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Index()
    {
        return View();
    }
}
