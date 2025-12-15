using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RedRiverTest.Api.Models;

namespace RedRiverTest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Enkel "fake-databas" i minnet
        private static readonly List<AppUser> _users = new();
        private static int _nextUserId = 1;

        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        public class RegisterRequest
        {
            public string UserName { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginRequest
        {
            public string UserName { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterRequest request)
        {
            var userName = request.UserName.Trim();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("UserName and Password are required");
            }

            if (_users.Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("User already exists");
            }

            var user = new AppUser
            {
                Id = _nextUserId++,
                UserName = userName,
                Password = request.Password   // DEMO: plaintext
            };

            _users.Add(user);

            return Ok();
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest request)
        {
            var userName = request.UserName.Trim();

            var user = _users.FirstOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(AppUser user)
        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutesStr = _config["Jwt:ExpireMinutes"];

            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("Jwt:Key is missing in appsettings.json");

            if (!int.TryParse(expireMinutesStr, out var expireMinutes))
            {
                expireMinutes = 60; // fallback
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // Viktigt: User.Identity.Name = user.UserName
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
