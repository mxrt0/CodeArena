using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Seeding;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}

