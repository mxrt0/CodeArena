using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class SubmissionsController : BaseAdminController
{
    private readonly IAdminSubmissionService _submissionService;
    public SubmissionsController(IAdminSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    public async Task<IActionResult> Index()
    {
        var submissions = await _submissionService.GetPendingSubmissionsAsync();
        var vm = new SubmissionsIndexViewModel
        {
            Submissions = submissions
        };
        return View(vm);
    }
}
