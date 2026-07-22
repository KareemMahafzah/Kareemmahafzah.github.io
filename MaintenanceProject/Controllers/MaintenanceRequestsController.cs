
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceProject.Models;
using AutoMapper;
using MaintenanceProject.Data;

[Microsoft.AspNetCore.Authorization.Authorize]
public class MaintenanceRequestsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MaintenanceRequestsController> _logger;
    private readonly IMapper _mapper;

    public MaintenanceRequestsController(ApplicationDbContext context, ILogger<MaintenanceRequestsController> logger, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }

    // GET: MAINTENANCEREQUESTS
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, string? sortBy = "date", string? sortDir = "desc")
    {
        _logger.LogInformation("Fetching maintenance requests (search={Search}, page={Page}, pageSize={PageSize}, sort={SortBy} {SortDir})", search, page, pageSize, sortBy, sortDir);

        var query = _context.MaintenanceRequests.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.Description.Contains(search));
        }

        // Apply sorting
        bool descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        switch ((sortBy ?? "date").ToLowerInvariant())
        {
            case "description":
                query = descending ? query.OrderByDescending(r => r.Description) : query.OrderBy(r => r.Description);
                break;
            case "completed":
                query = descending ? query.OrderByDescending(r => r.IsCompleted) : query.OrderBy(r => r.IsCompleted);
                break;
            case "date":
            default:
                query = descending ? query.OrderByDescending(r => r.RequestDate) : query.OrderBy(r => r.RequestDate);
                break;
        }

        var total = await query.CountAsync();
        var totalPages = (int)System.Math.Ceiling((double)total / Math.Max(1, pageSize));

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var vm = new MaintenanceProject.Models.MaintenanceRequestIndexViewModel
        {
            Items = items,
            SearchTerm = search,
            Page = page,
            TotalPages = totalPages
            ,
            SortBy = sortBy,
            SortDir = sortDir
            ,
            PageSize = pageSize
        };

        return View("Index", vm);
    }

    // GET: MAINTENANCEREQUESTS/Details/5
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Details called with null id");
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (maintenancerequest == null)
        {
            _logger.LogWarning("Maintenance request with id {Id} not found", id);
            return NotFound();
        }

        return View(maintenancerequest);
    }

    // GET: MAINTENANCEREQUESTS/Create
    public IActionResult Create()
    {
        _logger.LogInformation("Returning Create view");
        var vm = new MaintenanceRequestCreateViewModel { RequestDate = DateTime.Now };
        return View(vm);
    }

    // POST: MAINTENANCEREQUESTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaintenanceRequestCreateViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var maintenancerequest = _mapper.Map<MaintenanceRequest>(vm);

            _context.Add(maintenancerequest);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created maintenance request with id {Id}", maintenancerequest.Id);
            TempData["ToastMessage"] = "Maintenance request created.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogWarning("Create model state invalid");
        return View(vm);
    }

    // GET: MAINTENANCEREQUESTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            _logger.LogWarning("Edit GET called with null id");
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests.FindAsync(id);
        if (maintenancerequest == null)
        {
            _logger.LogWarning("Maintenance request with id {Id} not found for edit", id);
            return NotFound();
        }

        var vm = _mapper.Map<MaintenanceRequestEditViewModel>(maintenancerequest);

        _logger.LogInformation("Returning Edit view for id {Id}", id);
        return View(vm);
    }

    // POST: MAINTENANCEREQUESTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, MaintenanceRequestEditViewModel vm)
    {
        if (id != vm.Id)
        {
            _logger.LogWarning("Edit POST id mismatch: route id {RouteId} != model id {ModelId}", id, vm.Id);
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var entity = await _context.MaintenanceRequests.FindAsync(vm.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Maintenance request with id {Id} not found on Edit POST", vm.Id);
                    return NotFound();
                }
                // Map the changed fields from the view model to the entity
                _mapper.Map(vm, entity);

                _context.Update(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated maintenance request with id {Id}", vm.Id);
                TempData["ToastMessage"] = "Maintenance request updated.";
                TempData["ToastType"] = "success";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating maintenance request {Id}", vm.Id);
                if (!MaintenanceRequestExists(vm.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        _logger.LogWarning("Edit model state invalid for id {Id}", vm.Id);
        return View(vm);
    }

    // GET: MAINTENANCEREQUESTS/Delete/5
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var maintenancerequest = await _context.MaintenanceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (maintenancerequest == null)
        {
            return NotFound();
        }

        return View(maintenancerequest);
    }

    // POST: MAINTENANCEREQUESTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var maintenancerequest = await _context.MaintenanceRequests.FindAsync(id);
        if (maintenancerequest != null)
        {
                _context.MaintenanceRequests.Remove(maintenancerequest);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted maintenance request with id {Id}", id);
                TempData["ToastMessage"] = "Maintenance request deleted.";
                TempData["ToastType"] = "success";
        }

        // If the request is AJAX, return JSON so client can handle without full reload
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true, id });
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: MAINTENANCEREQUESTS/DeleteAjax/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAjax(int id)
    {
        var maintenancerequest = await _context.MaintenanceRequests.FindAsync(id);
        if (maintenancerequest == null)
        {
            return Json(new { success = false, message = "Not found" });
        }

        _context.MaintenanceRequests.Remove(maintenancerequest);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted maintenance request with id {Id} via AJAX", id);
        return Json(new { success = true, id });
    }

    private bool MaintenanceRequestExists(int? id)
    {
        return _context.MaintenanceRequests.Any(e => e.Id == id);
    }
}
