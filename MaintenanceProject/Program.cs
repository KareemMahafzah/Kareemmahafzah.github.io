using Microsoft.EntityFrameworkCore;
using MaintenanceProject.Data;
using MaintenanceProject.Mapping;
using MaintenanceProject.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add MVC support
builder.Services.AddControllersWithViews();
// Health checks
builder.Services.AddHealthChecks();
// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register ApplicationDbContext with SQL Server. Prefer a connection string from configuration,
// but also allow override via environment variable "DefaultConnection" for secrets management.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection")
               ?? Environment.GetEnvironmentVariable("DefaultConnection");

    if (string.IsNullOrEmpty(conn))
    {
        // Let EF throw a clear error if no connection string is configured at runtime.
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    options.UseSqlServer(conn);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Security headers middleware (configurable via appsettings)
app.UseSecurityHeaders();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// health endpoint
app.MapHealthChecks("/health");

// Apply pending EF migrations at startup (safe-guarded)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<MaintenanceProject.Data.ApplicationDbContext>();
    if (db != null)
    {
        db.Database.Migrate();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogError(ex, "An error occurred while migrating or initializing the database.");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose Program class for WebApplicationFactory in integration tests
public partial class Program { }
