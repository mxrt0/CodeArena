using CodeArena.Data.Common.Enums;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using static CodeArena.Common.OutputMessages;
using static CodeArena.Common.ApplicationConstants;
using CodeArena.Common.Exceptions;
using CodeArena.Services.QueryModels.Admin;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class ChallengesController : BaseAdminController
{
    const int PageSize = 12;
    private readonly IAdminChallengeService _challengeService;
    private readonly ILogger<ChallengesController> _logger;

    public ChallengesController(IAdminChallengeService challengeService,
        ILogger<ChallengesController> logger)
    {
        _challengeService = challengeService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(AdminChallengeQuery query)
    {
        query.Page = Math.Max(1, query.Page);
        query.PageSize = PageSize;

        var (challenges, count) = await _challengeService.GetChallengesAsync(query);

        var vm = new ChallengesIndexViewModel
        {
            Challenges = challenges,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(count / (double)PageSize),
            Search = query.Search,
            State = query.State
        };
        ViewData["ActivePage"] = "Challenges";
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
            return View(vm);
        }

        await _challengeService.CreateChallengeAsync(vm.Challenge);
        TempData[SuccessTempDataKey] = ChallengeCreatedMessage;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var challenge = await _challengeService.GetChallengeByIdAsync(id);

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
        catch (ChallengeNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CreateChallengeViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var existing = await _challengeService.GetChallengeByIdAsync(id);

            var editDto = new EditChallengeDto
            (
                Id: id,
                Title: vm.Challenge.Title,
                Description: vm.Challenge.Description,
                Difficulty: vm.Challenge.Difficulty,
                Tags: vm.Challenge.Tags ?? string.Empty
            );

            await _challengeService.UpdateChallengeAsync(editDto);
            TempData[SuccessTempDataKey] = ChallengeUpdatedMessage;
        }
        catch (ChallengeNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message; 
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var challenge = await _challengeService.GetChallengeByIdAsync(id);

            await _challengeService.DeleteChallengeAsync(challenge.Id);
            TempData[SuccessTempDataKey] = ChallengeUpdatedMessage;
        }
        catch (ChallengeNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
        }
        catch (ChallengeAlreadyDeletedException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = Admin_ChallengeAlreadyDeletedMessage;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        try
        {
            var challenge = await _challengeService.GetChallengeByIdAsync(id);

            await _challengeService.RestoreChallengeAsync(challenge.Id);
            TempData[SuccessTempDataKey] = ChallengeRestoredMessage;
        }
        catch (ChallengeNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
        }
        catch (ChallengeAlreadyActiveException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = Admin_ChallengeAlreadyActiveMessage;
        }

        return RedirectToAction(nameof(Index));
    }

}
