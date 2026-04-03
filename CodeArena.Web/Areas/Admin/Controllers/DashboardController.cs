using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class DashboardController : BaseAdminController
{
    private readonly IAdminDashboardService _dashboardService;

    public DashboardController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync();
        var vm = new AdminDashboardViewModel
        {
            DashboardData = dashboardData
        };

        ViewData["ActivePage"] = "Dashboard";
        return View(vm);
    }
}
