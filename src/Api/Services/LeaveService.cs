using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public record LeaveBalance(int EmployeeId, decimal Entitlement, decimal Used, decimal Pending, decimal Available);

public class LeaveService(AcmeDb db)
{
    public static decimal DaysOf(LeaveRequest r) =>
        r.HalfDay == HalfDay.None ? WorkingDays.Count(r.StartDate, r.EndDate) : 0.5m;

    public async Task<LeaveBalance?> GetBalanceAsync(int employeeId)
    {
        var emp = await db.Employees.FindAsync(employeeId);
        if (emp is null) return null;
        var requests = await db.LeaveRequests.Where(r => r.EmployeeId == employeeId).ToListAsync();
        var used = requests.Where(r => r.Status == LeaveStatus.Approved).Sum(DaysOf);
        var pending = requests.Where(r => r.Status == LeaveStatus.Pending).Sum(DaysOf);
        return new LeaveBalance(employeeId, emp.AnnualEntitlement, used, pending, emp.AnnualEntitlement - used - pending);
    }

    public Task<List<LeaveRequest>> ListAsync(int? employeeId) =>
        db.LeaveRequests.Where(r => employeeId == null || r.EmployeeId == employeeId)
            .OrderByDescending(r => r.StartDate).ToListAsync();

    // TODO (Lab 1): create, approve and reject leave requests, including half-day leave.
    // See docs/feature-spec-leave.md.
}
