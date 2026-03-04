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
    public async Task<IActionResult> Create(CreateChallengeViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(vm);
        }
        await _challengeService.CreateChallengeAsync(vm.Challenge);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Index()
    {
        var vm = new ChallengesIndexViewModel
        {
            Challenges = await _challengeService.GetChallengesAsync()
        };
        return View(vm);
    }
}
