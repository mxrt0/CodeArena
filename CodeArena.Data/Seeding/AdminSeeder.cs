using CodeArena.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static CodeArena.Common.OutputMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Seeding;

public static class AdminSeeder
{
    public static async Task EnsureAdminUserExistsAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        string? AdminEmail = config["AdminUser:Email"];
        string? AdminPassword = config["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(AdminEmail) || string.IsNullOrWhiteSpace(AdminPassword))
            throw new InvalidOperationException(MissingAdminCredentialsMessage);

        var user = await userManager.FindByEmailAsync(AdminEmail);

        if (user is null)
        {
            user = new ApplicationUser
            {
                DisplayName = "Admin",
                NormalizedDisplayName = "ADMIN",
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
