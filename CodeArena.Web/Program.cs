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
// TODO: Extract filters into separate query wrapper class
// TODO: Optimize performance of queries and data retrieval
// TODO: Implement query filter extension method via query wrapper class(es)
// TODO: Add tag filtration in UI (chips) and implement in query filter
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
