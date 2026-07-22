using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MaintenanceProject.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var total = await _context.MaintenanceRequests.CountAsync();
            var completed = await _context.MaintenanceRequests.CountAsync(r => r.IsCompleted);
            var pending = total - completed;

            ViewData["Total"] = total;
            ViewData["Completed"] = completed;
            ViewData["Pending"] = pending;
            return View();
        }

        public async Task<IActionResult> ExportCsv()
        {
            var items = await _context.MaintenanceRequests.OrderByDescending(r => r.RequestDate).ToListAsync();
            var sb = new StringBuilder();
            sb.AppendLine("Id,Description,RequestDate,IsCompleted");
            foreach (var i in items)
            {
                sb.AppendLine($"{i.Id},\"{i.Description.Replace("\"", "\"\"")}\",{i.RequestDate:O},{i.IsCompleted}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "maintenance_requests.csv");
        }
    }
}
