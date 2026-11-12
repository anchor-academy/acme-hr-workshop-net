namespace Api.Data;

public record Holiday(DateOnly Date, string Name, string State);

/// <summary>Synthetic training list. Not an official public holiday calendar.</summary>
public static class Holidays
{
    public static readonly string[] States = ["ALL", "SEL", "KUL", "PNG"];

    public static IEnumerable<Holiday> For(int year) =>
    [
        new(new(year, 1, 1), "New Year's Day", "ALL"),
        new(new(year, 5, 1), "Labour Day", "ALL"),
        new(new(year, 8, 31), "National Day", "ALL"),
        new(new(year, 9, 16), "Malaysia Day", "ALL"),
        new(new(year, 12, 25), "Christmas Day", "ALL"),
        new(new(year, 12, 11), "Sultan of Selangor's Birthday", "SEL"),
        new(new(year, 2, 1), "Federal Territory Day", "KUL"),
        new(new(year, 7, 7), "George Town Heritage Day", "PNG"),
    ];
}
