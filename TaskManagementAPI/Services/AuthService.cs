using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //  Register User
        public async Task<User?> Register(string fullName, string email, string password, string role = "User")
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null;  // Email already exists

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,  //  Plain text password (not secure)
                Role = role  
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        //  Login User
        public async Task<object?> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null)
            {
                Console.WriteLine($"[LOGIN ERROR]: User with email '{email}' not found.");
                return null; 
            }
            if (user.Password != password)
            {
                Console.WriteLine($"[LOGIN ERROR]: Incorrect password for email '{email}'.");
                return null;
            }

            var token = GenerateJwtToken(user);
            return new { token, role = user.Role };
        }

        //  Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            string? jwtKey = _config["Jwt:Key"];
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];
            int tokenExpiryMinutes = _config.GetValue<int>("Jwt:ExpiryMinutes", 120);

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                Console.WriteLine("[JWT ERROR]: Missing JWT configuration in appsettings.json.");
                throw new InvalidOperationException("JWT configuration is missing.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique Token ID
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //  Validate JWT Token
        public ClaimsPrincipal? ValidateToken(string token)
        {
            string? jwtKey = _config["Jwt:Key"];
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                Console.WriteLine("[JWT ERROR]: Missing JWT configuration.");
                throw new InvalidOperationException("JWT configuration is missing.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No delay for expiration
                }, out SecurityToken validatedToken);

                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JWT ERROR]: {ex.Message}");
                return null;  // Invalid token
            }
        }

        //  Fetch Authenticated User Info
        public async Task<User?> GetUserInfo(string userId)
        {
            if (!int.TryParse(userId, out int id))
            {
                Console.WriteLine($"[USER INFO ERROR]: Invalid user ID format '{userId}'.");
                return null;
            }

            return await _context.Users.FindAsync(id);
        }
    }
}
