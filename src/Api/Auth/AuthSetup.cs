using Api.Data;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Auth;

public static class Roles
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string HR = "HR";
    public static readonly string[] All = [Employee, Manager, HR];
}

/// <summary>
/// Sign-in with the built-in ASP.NET Core Identity API endpoints: email + password, bearer token, roles.
/// Every controller needs a signed-in user unless it says [AllowAnonymous]. No email service is used.
/// </summary>
public static class AuthSetup
{
    // Identity maps these too. They need email or let anyone create an account, so they are switched off.
    static readonly string[] Closed = ["/auth/register", "/auth/forgotPassword", "/auth/resetPassword", "/auth/confirmEmail", "/auth/resendConfirmationEmail"];

    public static IServiceCollection AddAcmeAuth(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddIdentityApiEndpoints<IdentityUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 8;
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AcmeDb>();
        services.Configure<BearerTokenOptions>(IdentityConstants.BearerScheme, o => o.BearerTokenExpiration = TimeSpan.FromHours(8));
        services.AddScoped<AccessService>();
        return services;
    }

    /// <summary>Swagger: an Authorize button for the bearer token, and the closed routes hidden.</summary>
    public static void AddAcmeAuth(this SwaggerGenOptions o)
    {
        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer" });
        o.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
        });
        o.DocumentFilter<HideClosedRoutes>();
    }

    public static void MapAcmeAuth(this WebApplication app)
    {
        app.Use((ctx, next) => Closed.Any(p => ctx.Request.Path.Equals(p, StringComparison.OrdinalIgnoreCase))
            ? Results.NotFound().ExecuteAsync(ctx)
            : next());
        app.MapGroup("/auth").MapIdentityApi<IdentityUser>().WithTags("Auth");
    }

    class HideClosedRoutes : IDocumentFilter
    {
        public void Apply(OpenApiDocument doc, DocumentFilterContext context)
        {
            foreach (var path in doc.Paths.Keys.Where(k => Closed.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList())
                doc.Paths.Remove(path);
        }
    }
}
