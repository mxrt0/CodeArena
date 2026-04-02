using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CodeArena.Web.Controllers;

public class ProfileController : BaseController
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Stats()
    {
        var stats = await _userService.GetUserStatsAsync(UserId!);

        var vm = new UserStatsViewModel
        {
            Stats = stats
        };

        return View(vm);
    }
}
