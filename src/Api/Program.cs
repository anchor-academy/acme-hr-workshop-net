using Api.Auth;
using Api.Data;
using Api.Services;
using Microsoft.EntityFrameworkCore;

Api.DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
if (Environment.GetEnvironmentVariable("PORT") is { } port) builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Database: Render Postgres when DATABASE_URL is set (the postgres:// URL Render gives you), otherwise a local SQLite file.
var pg = PgUrl.ToNpgsql(builder.Configuration["DATABASE_URL"]);
builder.Services.AddDbContext<AcmeDb>(o =>
{
    if (!string.IsNullOrWhiteSpace(pg)) o.UseNpgsql(pg);
    else o.UseSqlite(builder.Configuration["SQLITE_PATH"] ?? "Data Source=acme-hr.db");
});
builder.Services.AddScoped<LeaveService>();
builder.Services.AddAcmeAi(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.AddAcmeAuth());
builder.Services.AddAcmeAuth();

var origins = (builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcmeDb>();
    db.Database.EnsureCreated();
    Seed.Run(db);
    SeedLogins.Run(db, builder.Configuration["SEED_USER_PASSWORD"]);
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapAcmeAuth();
app.MapControllers().RequireAuthorization(); // signed in by default; opt out with [AllowAnonymous]
app.Run();

public partial class Program { }
