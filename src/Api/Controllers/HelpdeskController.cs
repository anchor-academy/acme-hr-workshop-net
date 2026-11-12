using Api.Auth;
using Api.Helpdesk;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>The conversation so far. Who is asking comes from the token, never from the body.</summary>
public record ChatRequest(List<ChatTurn> Messages);

[ApiController, Route("helpdesk")]
public class HelpdeskController(HelpdeskAgent agent) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(ChatRequest req, CancellationToken ct)
    {
        if (req.Messages.Count == 0) return Problem(statusCode: 400, title: "No messages");
        return Ok(await agent.RunAsync(User.EmployeeId(), req.Messages, ct));
    }
}
