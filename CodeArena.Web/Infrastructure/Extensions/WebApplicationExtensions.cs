using CodeArena.Data;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Data.Seeding;
using CodeArena.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Web.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Seeds roles, the admin user, and challenge slugs in a single call.
    /// Safe to call on every startup — all seeders are idempotent.
    /// </summary>
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = sp.GetRequiredService<IConfiguration>();
        var challengeRepo = sp.GetRequiredService<IChallengeRepository>();

        await RoleSeeder.SeedAsync(roleManager);
        await AdminSeeder.EnsureAdminUserExistsAsync(userManager, configuration);
        await SlugSeeder.EnsureAllChallengesHaveSlugsAsync(challengeRepo);
    }

    /// <summary>
    /// Configures the standard HTTP pipeline: exception handling, HTTPS,
    /// static files, routing, auth, and status-code pages.
    /// </summary>
    public static WebApplication UseStandardPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler("/Home/Error/500");
        app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");

        return app;
    }

    /// <summary>
    /// Maps the three route patterns used by the app:
    /// area routes, challenge slug routes, and the default MVC route.
    /// </summary>
    public static WebApplication MapAppRoutes(this WebApplication app)
    {
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "challengeSlugs",
            pattern: "challenges/{slug}",
            defaults: new { controller = "Challenges", action = "Details" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        return app;
    }

    /// <summary>
    /// Maps SignalR Hubs used by the app.
    /// </summary>
    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<LeaderboardHub>("/hubs/leaderboard");

        return app;
    }
}
