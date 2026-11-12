using Api.Models;

namespace Api.Data;

/// <summary>Synthetic data only. No real people.</summary>
public static class Seed
{
    static readonly string[] First = ["Aina", "Ben", "Chen", "Dina", "Eric", "Farah", "Gopal", "Hana", "Irfan", "Jia", "Kumar", "Lina", "Ming", "Nadia", "Omar", "Priya", "Qi", "Rizal", "Sara", "Tan"];
    static readonly string[] Last = ["Lee", "Lim", "Wong", "Ismail", "Raj", "Tan", "Ng", "Hassan"];
    static readonly string[] Depts = ["Engineering", "QA", "Product", "HR", "Finance"];

    public static void Run(AcmeDb db)
    {
        if (db.Employees.Any()) return;
        var employees = new List<Employee>();
        for (var i = 1; i <= 40; i++)
        {
            var name = $"{First[(i - 1) % First.Length]} {Last[(i - 1) % Last.Length]}";
            employees.Add(new Employee
            {
                Id = i,
                Name = name,
                Email = $"{name.Replace(" ", ".").ToLowerInvariant()}{i}@acme-hr.example",
                Department = Depts[(i - 1) % Depts.Length],
                ManagerId = i <= 5 ? null : ((i - 1) % 5) + 1,
                AnnualEntitlement = i % 3 == 0 ? 18 : 14,
            });
        }
        db.Employees.AddRange(employees);
        var year = DateTime.UtcNow.Year;
        db.LeaveRequests.AddRange(
            new LeaveRequest { EmployeeId = 6, StartDate = new(year, 3, 2), EndDate = new(year, 3, 4), Status = LeaveStatus.Approved, Reason = "Family trip" },
            new LeaveRequest { EmployeeId = 7, StartDate = new(year, 4, 10), EndDate = new(year, 4, 10), HalfDay = HalfDay.AM, Status = LeaveStatus.Approved, Reason = "Clinic" },
            new LeaveRequest { EmployeeId = 8, StartDate = new(year, 5, 18), EndDate = new(year, 5, 22), Status = LeaveStatus.Pending, Reason = "Holiday" });
        db.Policies.AddRange(PolicyDocs.All);
        db.SaveChanges();
    }
}
