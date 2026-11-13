using Api.Auth;
using Api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>The signed-in employee and their roles. Sign in first with POST /auth/login.</summary>
[ApiController, Route("me")]
public class MeController(AcmeDb db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var e = await db.Employees.FindAsync(User.EmployeeId());
        if (e is null) return Problem(statusCode: 404, title: "No employee record for this sign-in");
        var roles = Roles.All.Where(User.IsInRole).ToArray(); // Employee, Manager, HR order
        return Ok(new { e.Id, e.Name, e.Email, e.Department, e.ManagerId, e.AnnualEntitlement, Roles = roles });
    }
}
