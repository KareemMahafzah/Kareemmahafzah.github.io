using System;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceProject.Models
{
    public class MaintenanceRequestCreateViewModel
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Completed?")]
        public bool IsCompleted { get; set; }
    }
}
