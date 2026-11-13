using System.Security.Claims;
using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public const string EmployeeIdClaim = "employee_id";

    /// <summary>The signed-in employee. Always take this from the token, never from the request body or query.</summary>
    public static int EmployeeId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(EmployeeIdClaim), out var id) ? id : 0;
}

/// <summary>
/// Who may see or act on whose data. Enforced here in C#, not in the UI and not in a prompt.
/// Employee: own data. Manager: own data and direct reports. HR: everyone.
/// </summary>
public class AccessService(AcmeDb db)
{
    public async Task<bool> CanSeeEmployeeAsync(ClaimsPrincipal user, int employeeId)
    {
        var me = user.EmployeeId();
        if (me == employeeId || user.IsInRole(Roles.HR)) return true;
        return await IsDirectReportAsync(me, employeeId);
    }

    /// <summary>Approve or reject: the requester's manager or HR, and never your own request.</summary>
    public async Task<bool> CanDecideLeaveAsync(ClaimsPrincipal user, LeaveRequest request)
    {
        var me = user.EmployeeId();
        if (me == request.EmployeeId) return false;
        return user.IsInRole(Roles.HR) || await IsDirectReportAsync(me, request.EmployeeId);
    }

    /// <summary>Employee ids this user may see. Null means everyone (HR).</summary>
    public async Task<List<int>?> VisibleEmployeeIdsAsync(ClaimsPrincipal user)
    {
        if (user.IsInRole(Roles.HR)) return null;
        var me = user.EmployeeId();
        var reports = await db.Employees.Where(e => e.ManagerId == me).Select(e => e.Id).ToListAsync();
        return [me, .. reports];
    }

    Task<bool> IsDirectReportAsync(int managerId, int employeeId) =>
        db.Employees.AnyAsync(e => e.Id == employeeId && e.ManagerId == managerId);
}
