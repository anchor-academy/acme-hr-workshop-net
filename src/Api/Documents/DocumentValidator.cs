using System.Globalization;

namespace Api.Documents;

/// <summary>Business rules checked in C#. The JSON schema guarantees the shape; these check the meaning.</summary>
public static class DocumentValidator
{
    public static List<string> Check(Extraction e)
    {
        var p = new List<string>();
        switch (e.Kind)
        {
            case "medical_certificate" when e.MedicalCertificate is { } mc:
                if (string.IsNullOrWhiteSpace(mc.EmployeeName)) p.Add("employee_name is empty");
                if (string.IsNullOrWhiteSpace(mc.Clinic)) p.Add("clinic is empty");
                if (!TryDate(mc.DateFrom, out var from)) p.Add("date_from is not yyyy-MM-dd");
                if (!TryDate(mc.DateTo, out var to)) p.Add("date_to is not yyyy-MM-dd");
                if (p.Count == 0)
                {
                    if (to < from) p.Add("date_to is before date_from");
                    else if (mc.Days != to.DayNumber - from.DayNumber + 1) p.Add($"days is {mc.Days} but the date range is {to.DayNumber - from.DayNumber + 1} days");
                }
                break;
            case "expense_claim" when e.ExpenseClaim is { } c:
                if (string.IsNullOrWhiteSpace(c.Vendor)) p.Add("vendor is empty");
                if (!TryDate(c.Date, out _)) p.Add("date is not yyyy-MM-dd");
                if (c.Currency.Length != 3) p.Add("currency must be a 3-letter code");
                if (c.Subtotal + c.Tax != c.Total) p.Add($"total {c.Total} does not equal subtotal {c.Subtotal} + tax {c.Tax}");
                if (c.Items.Count > 0 && c.Items.Sum(i => i.Amount) != c.Subtotal) p.Add("items do not add up to the subtotal");
                break;
            case "other":
                p.Add("not a medical certificate or expense claim");
                break;
            default:
                p.Add($"kind is {e.Kind} but its data is missing");
                break;
        }
        return p;
    }

    static bool TryDate(string s, out DateOnly d) => DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
}
