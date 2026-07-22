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
// Identity
builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

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
app.UseAuthentication();
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

// Seed admin user and roles if configured
try
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var adminEmail = config["AdminUser:Email"];
    var adminPassword = config["AdminUser:Password"];

    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(adminRole));
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new Microsoft.AspNetCore.Identity.IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(admin, adminRole))
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetService<ILogger<Program>>();
    logger?.LogError(ex, "An error occurred while seeding admin user/roles.");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose Program class for WebApplicationFactory in integration tests
public partial class Program { }
