using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AcmeDb(DbContextOptions<AcmeDb> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Policy> Policies => Set<Policy>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<LeaveRequest>().Property(x => x.HalfDay).HasConversion<string>();
        b.Entity<LeaveRequest>().Property(x => x.Status).HasConversion<string>();
        b.Entity<Employee>().Property(x => x.AnnualEntitlement).HasPrecision(5, 1);
    }
}
