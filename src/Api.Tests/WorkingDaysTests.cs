using Api.Services;

namespace Api.Tests;

public class WorkingDaysTests
{
    [Fact]
    public void Single_weekday_counts_one() =>
        Assert.Equal(1, WorkingDays.Count(new DateOnly(2026, 11, 18), new DateOnly(2026, 11, 18)));

    [Fact]
    public void Monday_to_Friday_counts_five() =>
        Assert.Equal(5, WorkingDays.Count(new DateOnly(2026, 11, 16), new DateOnly(2026, 11, 20)));

    [Fact]
    public void End_before_start_counts_zero() =>
        Assert.Equal(0, WorkingDays.Count(new DateOnly(2026, 11, 20), new DateOnly(2026, 11, 16)));

    // Known bug in the starter: leave that spans a weekend must not count Saturday and Sunday (POL-01).
    [Fact]
    public void Friday_to_Monday_counts_two_working_days() =>
        Assert.Equal(2, WorkingDays.Count(new DateOnly(2026, 11, 20), new DateOnly(2026, 11, 23)));
}
