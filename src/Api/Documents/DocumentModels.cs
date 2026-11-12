using System.Text.Json.Serialization;

namespace Api.Documents;

public enum DocumentStatus { Valid, NeedsReview }

public class StoredDocument
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string FileName { get; set; } = "";
    public string Kind { get; set; } = "";
    public string ExtractedJson { get; set; } = "";
    public DocumentStatus Status { get; set; }
    public string Problems { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public record MedicalCertificate(
    [property: JsonPropertyName("employee_name")] string EmployeeName,
    [property: JsonPropertyName("clinic")] string Clinic,
    [property: JsonPropertyName("date_from")] string DateFrom,
    [property: JsonPropertyName("date_to")] string DateTo,
    [property: JsonPropertyName("days")] int Days);

public record ClaimItem(
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("amount")] decimal Amount);

public record ExpenseClaim(
    [property: JsonPropertyName("claim_no")] string ClaimNo,
    [property: JsonPropertyName("vendor")] string Vendor,
    [property: JsonPropertyName("date")] string Date,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("subtotal")] decimal Subtotal,
    [property: JsonPropertyName("tax")] decimal Tax,
    [property: JsonPropertyName("total")] decimal Total,
    [property: JsonPropertyName("items")] List<ClaimItem> Items);

public record Extraction(
    [property: JsonPropertyName("kind")] string Kind,
    [property: JsonPropertyName("medical_certificate")] MedicalCertificate? MedicalCertificate,
    [property: JsonPropertyName("expense_claim")] ExpenseClaim? ExpenseClaim);

public record ExtractionResult(Extraction? Data, DocumentStatus Status, IReadOnlyList<string> Problems, int Attempts);
