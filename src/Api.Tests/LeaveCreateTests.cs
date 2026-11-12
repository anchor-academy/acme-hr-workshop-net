using Api.Models;
using Api.Services;

namespace Api.Tests;

public class LeaveCreateTests
{
    static readonly DateOnly Mon = new(2026, 11, 16);

    [Fact]
    public async Task Two_day_request_is_approved_immediately()
    {
        using var db = TestDb.Create();
        var r = await new LeaveService(db).CreateAsync(new(1, Mon, Mon.AddDays(1)));
        Assert.Equal(LeaveStatus.Approved, r.Status);
    }

    [Fact]
    public async Task Five_day_request_needs_manager()
    {
        using var db = TestDb.Create();
        var r = await new LeaveService(db).CreateAsync(new(1, Mon, Mon.AddDays(4)));
        Assert.Equal(LeaveStatus.Pending, r.Status);
    }

    [Fact]
    public async Task Half_day_deducts_half()
    {
        using var db = TestDb.Create();
        var svc = new LeaveService(db);
        await svc.CreateAsync(new(1, Mon, Mon, HalfDay.AM));
        Assert.Equal(0.5m, (await svc.GetBalanceAsync(1))!.Used);
    }

    [Fact]
    public async Task AM_and_PM_on_same_day_do_not_overlap()
    {
        using var db = TestDb.Create();
        var svc = new LeaveService(db);
        await svc.CreateAsync(new(1, Mon, Mon, HalfDay.AM));
        var pm = await svc.CreateAsync(new(1, Mon, Mon, HalfDay.PM));
        Assert.Equal(LeaveStatus.Approved, pm.Status);
    }

    [Fact]
    public async Task Overlap_is_rejected()
    {
        using var db = TestDb.Create();
        var svc = new LeaveService(db);
        await svc.CreateAsync(new(1, Mon, Mon.AddDays(1)));
        await Assert.ThrowsAsync<LeaveException>(() => svc.CreateAsync(new(1, Mon.AddDays(1), Mon.AddDays(2))));
    }

    [Fact]
    public async Task Half_day_over_two_dates_is_rejected()
    {
        using var db = TestDb.Create();
        await Assert.ThrowsAsync<LeaveException>(() => new LeaveService(db).CreateAsync(new(1, Mon, Mon.AddDays(1), HalfDay.PM)));
    }

    [Fact]
    public async Task Not_enough_balance_is_rejected()
    {
        using var db = TestDb.Create();
        await Assert.ThrowsAsync<LeaveException>(() => new LeaveService(db).CreateAsync(new(1, Mon, Mon.AddDays(25))));
    }

    [Fact]
    public async Task Only_pending_can_be_approved()
    {
        using var db = TestDb.Create();
        var svc = new LeaveService(db);
        var r = await svc.CreateAsync(new(1, Mon, Mon));
        await Assert.ThrowsAsync<LeaveException>(() => svc.ApproveAsync(r.Id));
    }

    [Fact]
    public async Task Pending_can_be_approved()
    {
        using var db = TestDb.Create();
        var svc = new LeaveService(db);
        var r = await svc.CreateAsync(new(1, Mon, Mon.AddDays(4)));
        Assert.Equal(LeaveStatus.Approved, (await svc.ApproveAsync(r.Id)).Status);
    }
}
