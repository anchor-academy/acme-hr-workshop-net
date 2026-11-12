using System.Net;
using System.Net.Http.Json;

namespace Api.Tests;

/// <summary>The leave endpoints over HTTP: who asks comes from the token, and only the manager or HR decides.</summary>
public class LeaveApiTests(ApiFactory api) : IClassFixture<ApiFactory>
{
    static object FiveDays(int week) => new { startDate = Monday(week), endDate = Monday(week).AddDays(4), halfDay = "None", reason = "Trip" };
    static DateOnly Monday(int week)
    {
        var d = new DateOnly(DateTime.UtcNow.Year + 1, 1, 5).AddDays(7 * week);
        while (d.DayOfWeek != DayOfWeek.Monday) d = d.AddDays(1);
        return d;
    }

    [Fact]
    public async Task Leave_is_created_for_the_signed_in_employee_even_if_the_body_names_someone_else()
    {
        var farah = await api.SignedInAs(People.Farah);
        var res = await farah.PostAsJsonAsync("/leave", new { employeeId = 7, startDate = Monday(1), endDate = Monday(1), halfDay = "AM", reason = "Clinic" });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Equal(6, (await res.Content.ReadFromJsonAsync<Row>())!.EmployeeId);
    }

    [Fact]
    public async Task Employee_cannot_approve_their_own_request_but_their_manager_can()
    {
        var farah = await api.SignedInAs(People.Farah);
        var created = await (await farah.PostAsJsonAsync("/leave", FiveDays(3))).Content.ReadFromJsonAsync<Row>();
        Assert.Equal("Pending", created!.Status);

        Assert.Equal(HttpStatusCode.Forbidden, (await farah.PostAsync($"/leave/{created.Id}/approve", null)).StatusCode);

        var aina = await api.SignedInAs(People.Aina);
        var approved = await aina.PostAsync($"/leave/{created.Id}/approve", null);
        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        Assert.Equal("Approved", (await approved.Content.ReadFromJsonAsync<Row>())!.Status);
    }

    [Fact]
    public async Task Manager_of_another_team_gets_403_and_HR_can_reject()
    {
        var farah = await api.SignedInAs(People.Farah);
        var created = await (await farah.PostAsJsonAsync("/leave", FiveDays(6))).Content.ReadFromJsonAsync<Row>();

        var otherManager = await api.SignedInAs("ben.lim2@acme-hr.example");
        Assert.Equal(HttpStatusCode.Forbidden, (await otherManager.PostAsync($"/leave/{created!.Id}/reject", null)).StatusCode);

        var hr = await api.SignedInAs(People.Dina);
        Assert.Equal(HttpStatusCode.OK, (await hr.PostAsync($"/leave/{created.Id}/reject", null)).StatusCode);
    }

    [Fact]
    public async Task Unknown_request_is_404()
    {
        var aina = await api.SignedInAs(People.Aina);
        Assert.Equal(HttpStatusCode.NotFound, (await aina.PostAsync("/leave/99999/approve", null)).StatusCode);
    }

    record Row(int Id, int EmployeeId, string Status);
}
