using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public record LeaveBalance(int EmployeeId, decimal Entitlement, decimal Used, decimal Pending, decimal Available);
public record CreateLeave(int EmployeeId, DateOnly StartDate, DateOnly EndDate, HalfDay HalfDay = HalfDay.None, string Reason = "");

public class LeaveException(string message) : Exception(message);

public class LeaveService(AcmeDb db)
{
    /// <summary>Requests longer than this many working days need a manager (POL-02).</summary>
    public const int AutoApproveMaxDays = 3;

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

    public async Task<LeaveRequest> CreateAsync(CreateLeave input)
    {
        if (input.EndDate < input.StartDate) throw new LeaveException("End date must not be before start date.");
        if (input.HalfDay != HalfDay.None)
        {
            if (input.StartDate != input.EndDate) throw new LeaveException("A half day must start and end on the same date.");
            if (!WorkingDays.IsWorkingDay(input.StartDate)) throw new LeaveException("A half day must be on a working day.");
        }
        var request = new LeaveRequest { EmployeeId = input.EmployeeId, StartDate = input.StartDate, EndDate = input.EndDate, HalfDay = input.HalfDay, Reason = input.Reason };
        var days = DaysOf(request);
        if (days <= 0) throw new LeaveException("The request has no working days.");

        var balance = await GetBalanceAsync(input.EmployeeId) ?? throw new LeaveException($"Employee {input.EmployeeId} not found.");
        if (balance.Available < days) throw new LeaveException($"Not enough leave: {days} requested, {balance.Available} available.");

        var others = await db.LeaveRequests
            .Where(r => r.EmployeeId == input.EmployeeId && r.Status != LeaveStatus.Rejected)
            .ToListAsync();
        if (others.Any(o => Overlaps(o, request))) throw new LeaveException("The request overlaps another leave request.");

        request.Status = days <= AutoApproveMaxDays ? LeaveStatus.Approved : LeaveStatus.Pending;
        db.LeaveRequests.Add(request);
        await db.SaveChangesAsync();
        return request;
    }

    public Task<LeaveRequest> ApproveAsync(int id) => DecideAsync(id, LeaveStatus.Approved);
    public Task<LeaveRequest> RejectAsync(int id) => DecideAsync(id, LeaveStatus.Rejected);

    async Task<LeaveRequest> DecideAsync(int id, LeaveStatus status)
    {
        var r = await db.LeaveRequests.FindAsync(id) ?? throw new LeaveException($"Leave request {id} not found.");
        if (r.Status != LeaveStatus.Pending) throw new LeaveException($"Only Pending requests can be changed. This one is {r.Status}.");
        r.Status = status;
        await db.SaveChangesAsync();
        return r;
    }

    static bool Overlaps(LeaveRequest a, LeaveRequest b)
    {
        if (a.EndDate < b.StartDate || b.EndDate < a.StartDate) return false;
        // Same single date, one AM and one PM: no overlap (POL-03).
        if (a.StartDate == a.EndDate && b.StartDate == b.EndDate && a.StartDate == b.StartDate
            && a.HalfDay != HalfDay.None && b.HalfDay != HalfDay.None && a.HalfDay != b.HalfDay) return false;
        return true;
    }
}
