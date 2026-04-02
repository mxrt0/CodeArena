using CodeArena.Data;
using CodeArena.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CodeArena.Common.ApplicationConstants;
using static CodeArena.Common.OutputMessages;
using System.Reflection;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Infrastructure.Services;

namespace CodeArena.Web.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the database context and ASP.NET Identity with configuration
    /// driven identity options from appsettings.json ("Identity" section).
    /// Also configures SignalR and in-memory caching support.
    /// </summary>
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(DefaultConnectionStringName)
            ?? throw new InvalidOperationException(DefaultConnectionStringNotFoundMessage);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(DataAssemblyName)));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddMemoryCache();
        builder.Services.AddSignalR();

        return builder;
    }

    /// <summary>
    /// Registers ASP.NET Identity and configures its options from appsettings.json.
    /// Expects an "Identity" section — see appsettings.example.json for the full schema.
    /// </summary>
    public static WebApplicationBuilder AddIdentityConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var identitySection = builder.Configuration.GetSection("Identity");

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password
            var pwd = identitySection.GetSection("Password");
            options.Password.RequireDigit = pwd.GetValue("RequireDigit", false);
            options.Password.RequireUppercase = pwd.GetValue("RequireUppercase", false);
            options.Password.RequireNonAlphanumeric = pwd.GetValue("RequireNonAlphanumeric", false);
            options.Password.RequiredLength = pwd.GetValue("RequiredLength", 6);
            options.Password.RequiredUniqueChars = pwd.GetValue("RequiredUniqueChars", 1);
            options.Password.RequireLowercase = pwd.GetValue("RequireLowercase", false);

            // Lockout
            var lockout = identitySection.GetSection("Lockout");
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockout.GetValue("DefaultLockoutMinutes", 5));
            options.Lockout.MaxFailedAccessAttempts = lockout.GetValue("MaxFailedAccessAttempts", 5);
            options.Lockout.AllowedForNewUsers = lockout.GetValue("AllowedForNewUsers", true);

            // Sign-in
            var signIn = identitySection.GetSection("SignIn");
            options.SignIn.RequireConfirmedAccount = signIn.GetValue("RequireConfirmedAccount", false);
            options.SignIn.RequireConfirmedEmail = signIn.GetValue("RequireConfirmedEmail", false);
            options.SignIn.RequireConfirmedPhoneNumber = signIn.GetValue("RequireConfirmedPhoneNumber", false);

            // User
            var user = identitySection.GetSection("User");
            options.User.RequireUniqueEmail = user.GetValue("RequireUniqueEmail", true);
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            var cookie = identitySection.GetSection("Cookie");
            options.LoginPath = cookie.GetValue("LoginPath", "/Identity/Account/Login");
            options.LogoutPath = cookie.GetValue("LogoutPath", "/Identity/Account/Logout");
            options.AccessDeniedPath = cookie.GetValue("AccessDeniedPath", "/Home/Error/403");
            options.ExpireTimeSpan = TimeSpan.FromMinutes(cookie.GetValue("ExpireTimeSpan", 60));
            options.SlidingExpiration = cookie.GetValue("SlidingExpiration", true);
            options.Cookie.HttpOnly = true;
        });

        return builder;
    }

    /// <summary>
    /// Defines application cookie lifetime, configures paths and sets security flags based on the "Cookie" section of appsettings.json.
    /// </summary>
    public static WebApplicationBuilder AddAuthCookie(this WebApplicationBuilder builder)
    {
        var cookie = builder.Configuration.GetSection("Cookie");

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = cookie.GetValue("LoginPath", "/Identity/Account/Login");
            options.LogoutPath = cookie.GetValue("LogoutPath", "/Identity/Account/Logout");
            options.AccessDeniedPath = cookie.GetValue("AccessDeniedPath", "/Home/Error/403");
            options.ExpireTimeSpan = TimeSpan.FromMinutes(cookie.GetValue("ExpireTimeSpan", 60));
            options.SlidingExpiration = cookie.GetValue("SlidingExpiration", true);
            options.Cookie.HttpOnly = true;
        });

        return builder;
    }

    /// <summary>
    /// Scans the assembly containing <typeparamref name="TMarker"/> for all concrete
    /// classes that implement an interface whose name matches the pattern I{Name} → {Name},
    /// and registers them as Scoped. Restricts discovery to the provided interface
    /// base types so unrelated interfaces are ignored.
    /// </summary>
    public static WebApplicationBuilder AddRepositories<TMarker>(
        this WebApplicationBuilder builder,
        string repositoryNamespace)
    {
        RegisterByConvention(
            builder.Services,
            typeof(TMarker).Assembly,
            repositoryNamespace,
            suffix: "Repository");

        return builder;
    }

    /// <summary>
    /// Scans the assembly containing <typeparamref name="TMarker"/> for all concrete
    /// service classes and registers them as Scoped by convention.
    /// Manually registers SignalR-related notification service.
    /// </summary>
    public static WebApplicationBuilder AddAppServices<TMarker>(
        this WebApplicationBuilder builder,
        string serviceNamespace)
    {
        RegisterByConvention(
            builder.Services,
            typeof(TMarker).Assembly,
            serviceNamespace,
            suffix: "Service");

        builder.Services.AddScoped<INotificationService, NotificationService>();

        return builder;
    }

    /// <summary>
    /// Adds MVC controllers with views, Razor Pages, and enforces lowercase URLs.
    /// </summary>
    public static WebApplicationBuilder AddMvcWithDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        return builder;
    }

    private static void RegisterByConvention(
        IServiceCollection services,
        Assembly assembly,
        string targetNamespace,
        string suffix)
    {
        var concreteTypes = assembly
            .GetExportedTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Namespace is not null &&
                t.Namespace.StartsWith(targetNamespace, StringComparison.Ordinal) &&
                t.Name.EndsWith(suffix, StringComparison.Ordinal));

        foreach (var implementation in concreteTypes)
        {
            var serviceInterface = implementation
                .GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementation.Name}");

            if (serviceInterface is null)
                continue;

            if (services.Any(d => d.ServiceType == serviceInterface))
                continue;

            services.AddScoped(serviceInterface, implementation);
        }
    }
}
