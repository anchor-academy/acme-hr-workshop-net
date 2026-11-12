using Api.Auth;
using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController, Route("health"), AllowAnonymous]
public class HealthController(AcmeDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ok = await db.Database.CanConnectAsync();
        return Ok(new { status = ok ? "ok" : "error", database = ok ? "database connected" : "database NOT connected", provider = db.Database.ProviderName });
    }
}

[ApiController, Route("employees")]
public class EmployeesController(AcmeDb db, LeaveService leave, AccessService access) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Employees.OrderBy(e => e.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) =>
        await db.Employees.FindAsync(id) is { } e ? Ok(e) : Problem(statusCode: 404, title: $"Employee {id} not found");

    [HttpGet("{id:int}/summary")]
    public async Task<IActionResult> Summary(int id) => Ok(new { employeeId = id, daysTakenThisYear = await leave.DaysTakenThisYearAsync(id) });

    [HttpGet("{id:int}/balance")]
    public async Task<IActionResult> Balance(int id)
    {
        if (!await access.CanSeeEmployeeAsync(User, id)) return Problem(statusCode: 403, title: "You can only see your own or your team's leave");
        return await leave.GetBalanceAsync(id) is { } b ? Ok(b) : Problem(statusCode: 404, title: $"Employee {id} not found");
    }
}

public record NewLeave(DateOnly StartDate, DateOnly EndDate, HalfDay HalfDay = HalfDay.None, string Reason = "");

[ApiController, Route("leave")]
public class LeaveController(LeaveService leave, AccessService access) : ControllerBase
{
    /// <summary>Leave you may see: your own, your direct reports' (managers), everyone's (HR).</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? employeeId)
    {
        if (employeeId is int id)
            return await access.CanSeeEmployeeAsync(User, id)
                ? Ok(await leave.ListAsync(id))
                : Problem(statusCode: 403, title: "You can only see your own or your team's leave");
        var visible = await access.VisibleEmployeeIdsAsync(User);
        return Ok((await leave.ListAsync(null)).Where(r => visible is null || visible.Contains(r.EmployeeId)));
    }

    /// <summary>Leave for the signed-in employee. The body has no employeeId: who asks comes from the token.</summary>
    [HttpPost]
    public Task<IActionResult> Create(NewLeave body) =>
        Run(() => leave.CreateAsync(new CreateLeave(User.EmployeeId(), body.StartDate, body.EndDate, body.HalfDay, body.Reason)));

    [HttpPost("{id:int}/approve")]
    public Task<IActionResult> Approve(int id) => Decide(id, () => leave.ApproveAsync(id));

    [HttpPost("{id:int}/reject")]
    public Task<IActionResult> Reject(int id) => Decide(id, () => leave.RejectAsync(id));

    /// <summary>Only the requester's manager or HR, never on their own request (AccessService).</summary>
    async Task<IActionResult> Decide(int id, Func<Task<LeaveRequest>> action)
    {
        var request = await leave.FindAsync(id);
        if (request is null) return Problem(statusCode: 404, title: $"Leave request {id} not found");
        if (!await access.CanDecideLeaveAsync(User, request))
            return Problem(statusCode: 403, title: "Only the employee's manager or HR can approve or reject this request");
        return await Run(action);
    }

    async Task<IActionResult> Run(Func<Task<LeaveRequest>> action)
    {
        try { return Ok(await action()); }
        catch (LeaveException e) { return Problem(statusCode: 400, title: "Leave request not valid", detail: e.Message); }
    }
}

[ApiController, Route("policies")]
public class PoliciesController(AcmeDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Policies.OrderBy(p => p.Id).Select(p => new { p.Id, p.Title }).ToListAsync());
}
