using System.Text.Json.Serialization;
using Bank_System.Extensions;
using Bank_System.Middleware;
using Business_Logic;
using Business_Logic.Seed;
using Business_Logic.Settings;
using Data_Access;

var builder = WebApplication.CreateBuilder(args);

// ---------- Data Access layer ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("The connection string 'DefaultConnection' was not found.");

builder.Services.AddDataAccess(connectionString);

// ---------- Business layer + JWT ----------
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("The 'Jwt' section is missing from the configuration.");

builder.Services.AddBusinessServices(jwtSettings);
builder.Services.AddJwtAuthentication(jwtSettings);

// ---------- API (enums are sent/received as text: "Savings", "Deposit", ...) ----------
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

// must be the FIRST middleware so it catches every exception
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();   // always before UseAuthorization
app.UseAuthorization();
app.MapControllers();

await SeedDatabaseAsync(app);

app.Run();

// Roles, USD/JOD, head-office branch and the first admin user.
// Needs the tables to exist: run Update-Database first.
static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        var adminPassword = app.Configuration["Seed:AdminPassword"] ?? "Admin@12345";

        await seeder.SeedAsync(adminPassword);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Seeding was skipped. Did you run Update-Database?");
    }
}
