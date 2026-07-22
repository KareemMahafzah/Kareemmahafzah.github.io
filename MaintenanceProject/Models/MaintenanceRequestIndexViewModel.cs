using System.Collections.Generic;

namespace MaintenanceProject.Models
{
    public class MaintenanceRequestIndexViewModel
    {
        public IEnumerable<MaintenanceRequest> Items { get; set; } = new List<MaintenanceRequest>();
        public string? SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<int> PageSizeOptions { get; set; } = new[] { 5, 10, 20, 50 };
    }
}
