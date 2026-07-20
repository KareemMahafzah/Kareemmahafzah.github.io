using Microsoft.EntityFrameworkCore;
using MaintenanceProject.Models;

namespace MaintenanceProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
    }
}
