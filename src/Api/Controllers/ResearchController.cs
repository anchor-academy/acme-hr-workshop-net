using Api.Research;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public record ResearchRequest(string Question);

[ApiController, Route("research")]
public class ResearchController(PolicyResearch research) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Ask(ResearchRequest req, CancellationToken ct) =>
        string.IsNullOrWhiteSpace(req.Question) ? Problem(statusCode: 400, title: "Question is empty") : Ok(await research.AskAsync(req.Question, ct));
}
