using Api.Data;
using Api.Services;
using Microsoft.EntityFrameworkCore;

Api.DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
if (Environment.GetEnvironmentVariable("PORT") is { } port) builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Database: Supabase Postgres when SUPABASE_DB_URL is set, otherwise a local SQLite file.
var pg = builder.Configuration["SUPABASE_DB_URL"];
builder.Services.AddDbContext<AcmeDb>(o =>
{
    if (!string.IsNullOrWhiteSpace(pg)) o.UseNpgsql(pg);
    else o.UseSqlite(builder.Configuration["SQLITE_PATH"] ?? "Data Source=acme-hr.db");
});
builder.Services.AddScoped<LeaveService>();
builder.Services.AddSingleton<HolidayService>();
builder.Services.AddAcmeAi(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var origins = (builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcmeDb>();
    db.Database.EnsureCreated();
    SchemaUpgrade.AddMissingTables(db);
    Seed.Run(db);
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

public partial class Program { }
