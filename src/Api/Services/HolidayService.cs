using Api.Data;

namespace Api.Services;

public class HolidayService
{
    public IReadOnlyList<Holiday> List(string state, int year)
    {
        state = state.ToUpperInvariant();
        if (!Holidays.States.Contains(state)) throw new ArgumentException($"Unknown state '{state}'. Use one of: {string.Join(", ", Holidays.States)}.");
        return Holidays.For(year).Where(h => h.State == "ALL" || h.State == state).OrderBy(h => h.Date).ToList();
    }
}
