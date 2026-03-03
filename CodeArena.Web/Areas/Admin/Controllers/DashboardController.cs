using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class DashboardController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
