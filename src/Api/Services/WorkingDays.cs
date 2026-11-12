namespace Api.Services;

public static class WorkingDays
{
    /// <summary>Counts Monday–Friday days between start and end, inclusive (POL-01).</summary>
    public static int Count(DateOnly start, DateOnly end)
    {
        if (end < start) return 0;
        var days = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
            if (IsWorkingDay(d)) days++;
        return days;
    }

    public static bool IsWorkingDay(DateOnly d) => d.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
}
