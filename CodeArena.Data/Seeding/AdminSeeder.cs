using CodeArena.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Seeding;

public static class AdminSeeder
{
    public static async Task EnsureAdminUserExistsAsync(UserManager<ApplicationUser> userManager)
    {
        const string AdminEmail = "admin@codearena.com";
        const string AdminPassword = "as468@Aq4nv6EKy";

        var user = await userManager.FindByEmailAsync(AdminEmail);

        if (user is null)
        {
            user = new ApplicationUser
            {
                DisplayName = "Admin",
                Email = AdminEmail,
                UserName = AdminEmail,
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(user, AdminPassword);
        }

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
