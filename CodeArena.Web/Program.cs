using CodeArena.Data;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Data.Seeding;
using CodeArena.Services.Core;
using CodeArena.Services.Core.Admin;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Infrastructure.Extensions;
using static CodeArena.Common.ApplicationConstants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// TODO: Optimize performance of queries and data retrieval
namespace CodeArena.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder
            .AddInfrastructure()
            .AddIdentityConfiguration()
            .AddAuthCookie()
            .AddRepositories<IChallengeRepository>(RepositoriesNamespace)
            .AddAppServices<ISubmissionService>(ServicesNamespace)
            .AddMvcWithDefaults();

        var app = builder.Build();

        await app.SeedDatabaseAsync();

        app.UseStandardPipeline();
        app.MapAppRoutes();

        app.Run();
    }
}
