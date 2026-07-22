using Microsoft.EntityFrameworkCore;
using MaintenanceProject.Data;
using MaintenanceProject.Mapping;
using MaintenanceProject.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add MVC support
builder.Services.AddControllersWithViews();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose Program class for WebApplicationFactory in integration tests
public partial class Program { }
