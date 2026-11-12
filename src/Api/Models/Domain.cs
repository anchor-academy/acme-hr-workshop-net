namespace Api.Models;

public enum HalfDay { None, AM, PM }
public enum LeaveStatus { Pending, Approved, Rejected }

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public int? ManagerId { get; set; }
    public decimal AnnualEntitlement { get; set; } = 14;
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public HalfDay HalfDay { get; set; } = HalfDay.None;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string Reason { get; set; } = "";
}

public class Policy
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
}
