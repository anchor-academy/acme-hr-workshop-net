using Api.Auth;
using Microsoft.AspNetCore.Identity;

namespace Api.Data;

/// <summary>
/// One sign-in per seeded employee: their work email plus one shared training password.
/// Roles: everyone is Employee; anyone with direct reports is Manager; the HR department is HR.
/// Synthetic training data only. A real system never shares a password between users.
/// </summary>
public static class SeedLogins
{
    public const string DefaultPassword = "Acme@2026";

    public static void Run(AcmeDb db, string? password = null)
    {
        if (db.Users.Any()) return;

        var roles = Roles.All.ToDictionary(r => r, r => new IdentityRole(r) { NormalizedName = r.ToUpperInvariant() });
        db.Roles.AddRange(roles.Values);

        var hash = new PasswordHasher<IdentityUser>().HashPassword(new IdentityUser(), password ?? DefaultPassword);
        var employees = db.Employees.OrderBy(e => e.Id).ToList();
        foreach (var e in employees)
        {
            var user = new IdentityUser
            {
                UserName = e.Email,
                NormalizedUserName = e.Email.ToUpperInvariant(),
                Email = e.Email,
                NormalizedEmail = e.Email.ToUpperInvariant(),
                EmailConfirmed = true,
                LockoutEnabled = true,
                PasswordHash = hash,
                SecurityStamp = Guid.NewGuid().ToString("N"),
            };
            db.Users.Add(user);
            db.UserClaims.Add(new IdentityUserClaim<string> { UserId = user.Id, ClaimType = ClaimsPrincipalExtensions.EmployeeIdClaim, ClaimValue = e.Id.ToString() });

            var userRoles = new List<string> { Roles.Employee };
            if (employees.Any(x => x.ManagerId == e.Id)) userRoles.Add(Roles.Manager);
            if (e.Department == "HR") userRoles.Add(Roles.HR);
            foreach (var r in userRoles) db.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = roles[r].Id });
        }
        db.SaveChanges();
    }
}
