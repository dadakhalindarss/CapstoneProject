using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TaskManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        //  Register a new user (No Password Hashing -  Insecure)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Register user without password hashing
            var user = await _authService.Register(request.FullName, request.Email, request.Password, request.Role);
            if (user == null)
                return Conflict(new { message = "Email already in use" });

            return Ok(new { message = " Registration successful! You can now log in." });
        }

        //  Login and return JWT token with role
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loginResponse = await _authService.Login(request.Email, request.Password);
            if (loginResponse == null)
                return Unauthorized(new { message = " Invalid email or password" });

            return Ok(loginResponse); // Returns { token, role }
        }
    }

    //  DTOs with Validation
    public record RegisterRequest(
        [Required] string FullName,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password, //  Plain Text Password
        [Required] string Role
    );

    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password
    );
}
