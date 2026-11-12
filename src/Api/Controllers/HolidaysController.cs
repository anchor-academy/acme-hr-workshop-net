using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController, Route("holidays")]
public class HolidaysController(HolidayService holidays) : ControllerBase
{
    [HttpGet]
    public IActionResult List([FromQuery] string state = "ALL", [FromQuery] int? year = null)
    {
        try { return Ok(holidays.List(state, year ?? DateTime.UtcNow.Year)); }
        catch (ArgumentException e) { return Problem(statusCode: 400, title: "Unknown state", detail: e.Message); }
    }
}
