using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using usermangement.Data;
using usermangement.Models;

namespace usermangement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        return BadRequest(new { message = "Name, email, and password are required." });

    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return BadRequest(new { message = "Email already exists." });

    var newUser = new User
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Email = request.Email,
        CreatedAt = DateTime.UtcNow,
        LastLoginTime = DateTime.UtcNow,
        IsBlocked = false
    };

    // Hash password correctly
    newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);
    Console.WriteLine($"New User Password Hash: {newUser.PasswordHash}"); // Debugging

    _context.Users.Add(newUser);
    await _context.SaveChangesAsync();

    return Ok(new { message = "User registered successfully." });
}

      [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        return BadRequest(new { message = "Email and password are required." });

    var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
    if (user == null)
    {
        return Unauthorized(new { message = "Invalid credentials. (User not found)" });
    }

    // Debugging logs
    Console.WriteLine($"Stored Hash in DB: {user.PasswordHash}");
    Console.WriteLine($"Input Password: {request.Password}");

    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
    Console.WriteLine($"Password Verification Result: {result}"); // Debugging

    if (result == PasswordVerificationResult.Failed)
    {
        return Unauthorized(new { message = "Invalid credentials. (Password mismatch)" });
    }

    if (user.IsBlocked)
        return Unauthorized(new { message = "Your account is blocked. Contact admin." });

    user.LastLoginTime = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    var token = GenerateJwtToken(user);
    return Ok(new { token });
}


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
