using Api.Data;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController, Route("health")]
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
public class EmployeesController(AcmeDb db, LeaveService leave) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Employees.OrderBy(e => e.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) =>
        await db.Employees.FindAsync(id) is { } e ? Ok(e) : Problem(statusCode: 404, title: $"Employee {id} not found");

    [HttpGet("{id:int}/balance")]
    public async Task<IActionResult> Balance(int id) =>
        await leave.GetBalanceAsync(id) is { } b ? Ok(b) : Problem(statusCode: 404, title: $"Employee {id} not found");
}

[ApiController, Route("leave")]
public class LeaveController(LeaveService leave) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? employeeId) => Ok(await leave.ListAsync(employeeId));

    // TODO (Lab 1): POST /leave, POST /leave/{id}/approve, POST /leave/{id}/reject
}

[ApiController, Route("policies")]
public class PoliciesController(AcmeDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Policies.OrderBy(p => p.Id).Select(p => new { p.Id, p.Title }).ToListAsync());
}
