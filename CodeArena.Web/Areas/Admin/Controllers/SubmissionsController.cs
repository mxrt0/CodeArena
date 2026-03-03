using CodeArena.Services.Core.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class SubmissionsController : BaseAdminController
{
    private readonly IAdminSubmissionService _submissionService;
    public SubmissionsController(IAdminSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    public async Task<IActionResult> Pending()
    {
        var submissions = await _submissionService.GetPendingSubmissionsAsync();
        return View(submissions);
    }
}
