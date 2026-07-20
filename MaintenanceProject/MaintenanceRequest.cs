using System;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceProject.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        [Required]
        public string Priority { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }
}