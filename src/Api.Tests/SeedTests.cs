namespace Api.Tests;

public class SeedTests
{
    [Fact]
    public void Seeds_forty_employees()
    {
        using var db = TestDb.Create();
        Assert.Equal(40, db.Employees.Count());
    }

    [Fact]
    public void Seeds_fifteen_policies()
    {
        using var db = TestDb.Create();
        Assert.Equal(15, db.Policies.Count());
    }

    [Fact]
    public void Seed_is_idempotent()
    {
        using var db = TestDb.Create();
        Api.Data.Seed.Run(db);
        Assert.Equal(40, db.Employees.Count());
    }
}
