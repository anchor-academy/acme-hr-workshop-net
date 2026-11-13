using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>App data plus the ASP.NET Core Identity sign-in tables (AspNetUsers and friends).</summary>
public class AcmeDb(DbContextOptions<AcmeDb> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Policy> Policies => Set<Policy>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b); // sign-in tables (ASP.NET Core Identity)
        b.Entity<LeaveRequest>().Property(x => x.HalfDay).HasConversion<string>();
        b.Entity<LeaveRequest>().Property(x => x.Status).HasConversion<string>();
        b.Entity<Employee>().Property(x => x.AnnualEntitlement).HasPrecision(5, 1);
    }
}
