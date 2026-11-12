using System.Text.Json;
using Api.Data;
using Api.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public record DocumentUpload(int EmployeeId, string FileName, string Text);
public record ExtractRequest(string Text);

[ApiController, Route("documents")]
public class DocumentsController(DocumentExtractor extractor, AcmeDb db) : ControllerBase
{
    public const int MaxChars = 20_000;

    /// <summary>Extract only, nothing saved. Used by tools/ExtractScore.</summary>
    [HttpPost("extract")]
    public async Task<IActionResult> Extract(ExtractRequest req, CancellationToken ct)
    {
        if (Invalid(req.Text) is { } p) return p;
        return Ok(await extractor.ExtractAsync(req.Text, ct));
    }

    /// <summary>Intake: extract, check, save. Invalid documents are saved as NeedsReview for a person.</summary>
    [HttpPost]
    public async Task<IActionResult> Upload(DocumentUpload req, CancellationToken ct)
    {
        if (Invalid(req.Text) is { } p) return p;
        if (!await db.Employees.AnyAsync(e => e.Id == req.EmployeeId, ct)) return Problem(statusCode: 404, title: "Employee not found");
        var r = await extractor.ExtractAsync(req.Text, ct);
        var doc = new StoredDocument
        {
            EmployeeId = req.EmployeeId,
            FileName = string.IsNullOrWhiteSpace(req.FileName) ? "pasted.txt" : req.FileName,
            Kind = r.Data?.Kind ?? "unknown",
            ExtractedJson = r.Data is null ? "" : JsonSerializer.Serialize(r.Data),
            Status = r.Status,
            Problems = string.Join("; ", r.Problems),
        };
        db.Documents.Add(doc);
        await db.SaveChangesAsync(ct);
        return Created($"/documents/{doc.Id}", doc);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] DocumentStatus? status, CancellationToken ct) =>
        Ok(await db.Documents.Where(d => status == null || d.Status == status).OrderByDescending(d => d.Id).Take(100).ToListAsync(ct));

    IActionResult? Invalid(string? text) =>
        string.IsNullOrWhiteSpace(text) ? Problem(statusCode: 400, title: "Document text is empty")
        : text.Length > MaxChars ? Problem(statusCode: 400, title: $"Document is longer than {MaxChars} characters")
        : null;
}
