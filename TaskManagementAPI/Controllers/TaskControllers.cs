using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Get all tasks (Users can view)
        [HttpGet]
        [Authorize] // Requires authentication
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.Tasks.Include(t => t.Assignee).ToListAsync();
            return Ok(tasks);
        }

        // 🔹 Create a new task (Only Admin & Manager can create)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")] // Restrict access
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            if (task == null || string.IsNullOrEmpty(task.Title))
                return BadRequest("Task title is required.");

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }

        // 🔹 Update task details (Only Admin & Manager can update)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Restrict access
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound("Task not found.");

            task.Title = updatedTask.Title ?? task.Title;
            task.Description = updatedTask.Description ?? task.Description;
            task.Status = updatedTask.Status;

            await _context.SaveChangesAsync();
            return Ok(task);
        }

        // 🔹 Assign task to a user (Only Admin & Manager)
        [HttpPut("{taskId}/assign/{userId}")]
        [Authorize(Roles = "Admin,Manager")] // Restrict access
        public async Task<IActionResult> AssignTask(int taskId, int userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            var user = await _context.Users.FindAsync(userId);

            if (task == null) return NotFound("Task not found.");
            if (user == null) return NotFound("User not found.");

            task.AssigneeId = userId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task assigned successfully", task });
        }

        // 🔹 Get User Role from JWT Token
        private string GetUserRole()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";
            }
            return "User";
        }
    }
}
