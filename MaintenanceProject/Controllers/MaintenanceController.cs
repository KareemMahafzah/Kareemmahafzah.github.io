using Microsoft.AspNetCore.Mvc;
using MaintenanceProject.Models;
using MaintenanceProject.Data;
using Microsoft.EntityFrameworkCore;


namespace MaintenanceProject.Controllers
{
    private readonly ApplicationDbContext _context;

    public MaintenanceController()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MaintenanceDB;Trusted_Connection=True;");
        _context = new ApplicationDbContext(optionsBuilder.Options);
    }

    public class MaintenanceController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(MaintenanceRequest request)
        {
            if (ModelState.IsValid)
            {
                _context.MaintenanceRequests.Add(request);
                _context.SaveChanges();
                ViewBag.Message = $"Request submitted by {request.Name} — Issue: \"{request.IssueDescription}\" (Priority: {request.Priority})";
            }
            return View("MaintenanceProject");
        }
    }
}
