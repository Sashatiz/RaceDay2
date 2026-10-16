
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RaceDay.API;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(
            RaceDayDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public class RegisterRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Role { get; set; } = "Participant";
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    Message = "Full name, email and password are required."
                });
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(new
                {
                    Message = "Password must be at least 8 characters long."
                });
            }

            var email = request.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return Conflict(new
                {
                    Message = "Email is already registered."
                });
            }

            // Public registration only allows Participant accounts.
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Participant");

            if (role == null)
            {
                return BadRequest(new
                {
                    Message = "Participant role was not found."
                });
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                PhoneNumber = request.PhoneNumber.Trim(),
                RoleID = role.RoleID
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "Registration successful.",
                user.UserID,
                user.FullName,
                user.Email,
                Role = role.RoleName
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    Message = "Email and password are required."
                });
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            var verification =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleID == user.RoleID);

            if (role == null)
            {
                return Unauthorized(new
                {
                    Message = "User role was not found."
                });
            }

            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new
                {
                    Message = "JWT configuration is incomplete."
                });
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName),

                new Claim(
                    ClaimTypes.Role,
                    role.RoleName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Message = "Login successful.",
                Token = tokenString,
                user.UserID,
                user.FullName,
                user.Email,
                Role = role.RoleName
            });
        }
    }
}
