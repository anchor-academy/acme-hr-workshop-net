using Api.Services;

namespace Api.Tests;

public class HolidayServiceTests
{
    readonly HolidayService _svc = new();

    [Fact]
    public void National_list_has_only_national_days() =>
        Assert.All(_svc.List("ALL", 2026), h => Assert.Equal("ALL", h.State));

    [Fact]
    public void State_list_adds_state_days() =>
        Assert.Contains(_svc.List("sel", 2026), h => h.State == "SEL");

    [Fact]
    public void Unknown_state_throws() =>
        Assert.Throws<ArgumentException>(() => _svc.List("XYZ", 2026));
}
