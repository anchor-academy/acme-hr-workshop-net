using Api.Helpdesk;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public record ChatRequest(int EmployeeId, List<ChatTurn> Messages);

[ApiController, Route("helpdesk")]
public class HelpdeskController(HelpdeskAgent agent) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<IActionResult> Chat(ChatRequest req, CancellationToken ct)
    {
        if (req.Messages.Count == 0) return Problem(statusCode: 400, title: "No messages");
        return Ok(await agent.RunAsync(req.EmployeeId, req.Messages, ct));
    }
}
