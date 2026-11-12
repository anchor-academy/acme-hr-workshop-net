namespace Api.Services;

public static class WorkingDays
{
    /// <summary>Counts Monday–Friday days between start and end, inclusive.</summary>
    public static int Count(DateOnly start, DateOnly end)
    {
        if (end < start) return 0;
        // BUG (starter): weekends are not skipped. A test in LeaveServiceTests shows it.
        return end.DayNumber - start.DayNumber + 1;
    }
}
