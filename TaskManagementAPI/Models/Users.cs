using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string FullName { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty; // ✅ Using plain text password

        [Required]
        public string Role { get; set; } = "User"; // Default role

        public User()
        {
            FullName = string.Empty;
            Email = string.Empty;
            Password = string.Empty; // ✅ Ensure it initializes correctly
        }
    }
}
