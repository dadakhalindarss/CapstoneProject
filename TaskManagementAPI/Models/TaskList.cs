using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public required string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }  // Nullable to avoid warnings

        [Required, MaxLength(50)]
        public string Status { get; set; } = "To Do"; // Default status

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Auto-set timestamp

        public DateTime? UpdatedAt { get; set; } // Nullable, updated only when modified

        [ForeignKey("Assignee")]  // Correctly references the Assignee navigation property
        public int? AssigneeId { get; set; } // Nullable to allow unassigned tasks

        public User? Assignee { get; set; } // Navigation property for related User
    }
}
