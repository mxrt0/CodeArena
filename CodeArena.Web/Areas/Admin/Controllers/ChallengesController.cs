using CodeArena.Data.Common.Enums;
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

    public async Task<IActionResult> Index()
    {
        var vm = new ChallengesIndexViewModel
        {
            Challenges = await _challengeService.GetChallengesAsync()
        };
        return View(vm);
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var challenge = await _challengeService.GetChallengeByIdAsync(id);
        
        if (challenge is null)
        {
            return NotFound();
        }

        var challengeDto = new CreateChallengeDto
        {
            Title = challenge.Title,
            Description = challenge.Description,
            Difficulty = Enum.Parse<Difficulty>(challenge.Difficulty),
            Tags = string.Join(",", challenge.Tags)
        };
        var vm = new CreateChallengeViewModel
        {
            Challenge = challengeDto,
        };
        TempData["ChallengeId"] = id;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CreateChallengeViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var existing = await _challengeService.GetChallengeByIdAsync(id);
        if (existing is null)
            return NotFound();

        var editDto = new EditChallengeDto
        (
            Id: id,
            Title: vm.Challenge.Title,
            Description: vm.Challenge.Description,
            Difficulty: vm.Challenge.Difficulty,
            Tags: vm.Challenge.Tags ?? string.Empty
        );

        await _challengeService.UpdateChallengeAsync(editDto);

        return RedirectToAction(nameof(Index));
    }

}
