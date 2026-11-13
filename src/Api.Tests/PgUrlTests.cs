using Api.Data;
using Xunit;

namespace Api.Tests;

public class PgUrlTests
{
    [Fact]
    public void Render_internal_url_becomes_npgsql_keywords()
    {
        var cs = PgUrl.ToNpgsql("postgresql://acmehr:p%40ss@dpg-abc123-a/acmehr");
        Assert.Contains("Host=dpg-abc123-a", cs);
        Assert.Contains("Database=acmehr", cs);
        Assert.Contains("Username=acmehr", cs);
        Assert.Contains("Password=p@ss", cs);
        Assert.DoesNotContain("SSL Mode", cs);
    }

    [Fact]
    public void External_url_requires_ssl() =>
        Assert.Contains("SSL Mode=Require", PgUrl.ToNpgsql("postgres://u:p@dpg-x.singapore-postgres.render.com:5432/db"));

    [Fact]
    public void Keyword_strings_pass_through() =>
        Assert.Equal("Host=h;Database=d", PgUrl.ToNpgsql("Host=h;Database=d"));
}
