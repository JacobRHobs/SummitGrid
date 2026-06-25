using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SummitGrid.Api.Models;
using SummitGrid.Infrastructure;
using SummitGrid.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SummitGrid.Api.Controllers;
[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase
{
    private readonly SummitGridDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(SummitGridDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }   

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if(user == null) return Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized();
        

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), role = user.Role.ToString() });


    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if(String.IsNullOrEmpty(request.Username) || String.IsNullOrEmpty(request.Password))
            return BadRequest("Username and Password are required.");

        var exists = await _context.Users.AnyAsync(u => u.Username ==  request.Username);
        if(exists) return Conflict("Username already exists");
        
        var _user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(_user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Register), null);
    }

}
