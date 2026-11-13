using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Api.Auth;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests;

// Seeded people used below (see Seed.cs): 1 Aina Lee manages 6 Farah Tan (Engineering).
// 4 Dina Ismail is a manager in HR. 7 Gopal Ng is an employee in QA (manager 2).
static class People
{
    public const string Aina = "aina.lee1@acme-hr.example";   // Manager
    public const string Dina = "dina.ismail4@acme-hr.example"; // Manager + HR
    public const string Farah = "farah.tan6@acme-hr.example";  // Employee, reports to 1
    public const string Gopal = "gopal.ng7@acme-hr.example";   // Employee, reports to 2

    public static ClaimsPrincipal As(int employeeId, params string[] roles) => new(new ClaimsIdentity(
        [new Claim(ClaimsPrincipalExtensions.EmployeeIdClaim, employeeId.ToString()), .. roles.Select(r => new Claim(ClaimTypes.Role, r))], "test"));
}

public class SeedLoginsTests
{
    [Fact]
    public void Every_employee_gets_a_sign_in_with_the_training_password()
    {
        using var db = TestDb.Create();
        SeedLogins.Run(db);

        Assert.Equal(db.Employees.Count(), db.Users.Count());
        var user = db.Users.Single(u => u.Email == People.Farah);
        Assert.Equal(PasswordVerificationResult.Success,
            new PasswordHasher<IdentityUser>().VerifyHashedPassword(user, user.PasswordHash!, SeedLogins.DefaultPassword));
        Assert.True(user.LockoutEnabled);
    }

    [Theory]
    [InlineData(People.Farah, new[] { Roles.Employee })]
    [InlineData(People.Aina, new[] { Roles.Employee, Roles.Manager })]
    [InlineData(People.Dina, new[] { Roles.Employee, Roles.Manager, Roles.HR })]
    public void Roles_follow_the_org_chart(string email, string[] expected)
    {
        using var db = TestDb.Create();
        SeedLogins.Run(db);

        var userId = db.Users.Single(u => u.Email == email).Id;
        var roles = db.UserRoles.Where(ur => ur.UserId == userId).Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name!).ToList();
        Assert.Equal(expected.OrderBy(x => x), roles.OrderBy(x => x));
    }

    [Fact]
    public void Running_twice_does_not_duplicate_users()
    {
        using var db = TestDb.Create();
        SeedLogins.Run(db);
        SeedLogins.Run(db);
        Assert.Equal(db.Employees.Count(), db.Users.Count());
    }
}

public class AccessServiceTests
{
    [Fact]
    public async Task Employee_sees_own_data_only()
    {
        using var db = TestDb.Create();
        var access = new AccessService(db);
        var farah = People.As(6, Roles.Employee);
        Assert.True(await access.CanSeeEmployeeAsync(farah, 6));
        Assert.False(await access.CanSeeEmployeeAsync(farah, 7));
        Assert.Equal([6], await access.VisibleEmployeeIdsAsync(farah));
    }

    [Fact]
    public async Task Manager_sees_direct_reports_but_not_other_teams()
    {
        using var db = TestDb.Create();
        var access = new AccessService(db);
        var aina = People.As(1, Roles.Employee, Roles.Manager);
        Assert.True(await access.CanSeeEmployeeAsync(aina, 6));
        Assert.False(await access.CanSeeEmployeeAsync(aina, 7));
    }

    [Fact]
    public async Task HR_sees_everyone()
    {
        using var db = TestDb.Create();
        var access = new AccessService(db);
        var dina = People.As(4, Roles.Employee, Roles.Manager, Roles.HR);
        Assert.True(await access.CanSeeEmployeeAsync(dina, 7));
        Assert.Null(await access.VisibleEmployeeIdsAsync(dina));
    }

    [Fact]
    public async Task Only_the_manager_or_HR_decides_and_never_on_their_own_request()
    {
        using var db = TestDb.Create();
        var access = new AccessService(db);
        var farahs = new LeaveRequest { EmployeeId = 6 };
        var ainas = new LeaveRequest { EmployeeId = 1 };

        Assert.True(await access.CanDecideLeaveAsync(People.As(1, Roles.Manager), farahs));
        Assert.True(await access.CanDecideLeaveAsync(People.As(4, Roles.HR), farahs));
        Assert.False(await access.CanDecideLeaveAsync(People.As(6, Roles.Employee), farahs));
        Assert.False(await access.CanDecideLeaveAsync(People.As(2, Roles.Manager), farahs));
        Assert.False(await access.CanDecideLeaveAsync(People.As(1, Roles.Manager), ainas));
    }
}

/// <summary>A real API over HTTP with its own throwaway SQLite file.</summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    readonly string _file = Path.Combine(Path.GetTempPath(), $"acme-test-{Guid.NewGuid():N}.db");
    public ApiFactory() => Environment.SetEnvironmentVariable("SQLITE_PATH", $"Data Source={_file}");

    public async Task<HttpClient> SignedInAs(string email)
    {
        var http = CreateClient();
        var login = await http.PostAsJsonAsync("/auth/login", new { email, password = SeedLogins.DefaultPassword });
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return http;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        File.Delete(_file);
    }

    record TokenResponse(string AccessToken);
}

public class AuthApiTests(ApiFactory api) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Sign_in_then_me_returns_the_employee_and_roles()
    {
        var http = await api.SignedInAs(People.Aina);
        var me = await http.GetFromJsonAsync<MeResponse>("/me");
        Assert.Equal(1, me!.Id);
        Assert.Contains(Roles.Manager, me.Roles);
    }

    [Fact]
    public async Task Wrong_password_is_rejected()
    {
        var login = await api.CreateClient().PostAsJsonAsync("/auth/login", new { email = People.Aina, password = "wrong-password" });
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task Five_wrong_passwords_lock_the_account()
    {
        var http = api.CreateClient();
        for (var i = 0; i < 5; i++)
            await http.PostAsJsonAsync("/auth/login", new { email = People.Gopal, password = "wrong-password" });
        var right = await http.PostAsJsonAsync("/auth/login", new { email = People.Gopal, password = SeedLogins.DefaultPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, right.StatusCode);
    }

    [Theory]
    [InlineData("/me")]
    [InlineData("/employees")]
    [InlineData("/leave")]
    public async Task Endpoints_need_a_signed_in_user(string path)
    {
        var res = await api.CreateClient().GetAsync(path);
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Health_is_open()
    {
        var res = await api.CreateClient().GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Self_registration_is_switched_off()
    {
        var res = await api.CreateClient().PostAsJsonAsync("/auth/register", new { email = "new@acme-hr.example", password = "Whatever@2026" });
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task Employee_cannot_see_someone_elses_balance()
    {
        var http = await api.SignedInAs(People.Farah);
        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync("/employees/6/balance")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await http.GetAsync("/employees/7/balance")).StatusCode);
    }

    [Fact]
    public async Task Manager_sees_their_team_leave_only()
    {
        var http = await api.SignedInAs(People.Aina);
        var rows = await http.GetFromJsonAsync<List<LeaveRow>>("/leave");
        Assert.All(rows!, r => Assert.Contains(r.EmployeeId, new[] { 1, 6, 11, 16, 21, 26, 31, 36 }));
        Assert.Equal(HttpStatusCode.Forbidden, (await http.GetAsync("/leave?employeeId=7")).StatusCode);
    }

    record MeResponse(int Id, string Email, string[] Roles);
    record LeaveRow(int Id, int EmployeeId);
}
