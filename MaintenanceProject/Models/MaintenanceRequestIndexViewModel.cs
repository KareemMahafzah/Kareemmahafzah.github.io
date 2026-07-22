using System.Collections.Generic;

namespace MaintenanceProject.Models
{
    public class MaintenanceRequestIndexViewModel
    {
        public IEnumerable<MaintenanceRequest> Items { get; set; } = new List<MaintenanceRequest>();
        public string? SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
