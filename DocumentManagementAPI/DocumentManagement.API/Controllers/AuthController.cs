// API/Controllers/AuthController.cs
using DocumentManagement.Application.DTOs;
using DocumentManagement.Application.Services;
using DocumentManagement.Domain.Entities;
using DocumentManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly IPasswordHasher<User> _passwordHasher;
 
    public AuthController(AppDbContext context, AuthService authService, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }
 
   [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest userInput) // Use RegisterRequest
    {
        if (await _context.Users.AnyAsync(u => u.Username == userInput.Username))
            return BadRequest("Username already exists.");

        var user = new User
        {
            Username = userInput.Username,
            Email = userInput.Email,  // Save email in User entity
            Role = userInput.Role
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userInput.PasswordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }
 
     // Login endpoint accepting only username and password
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginInput)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginInput.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginInput.PasswordHash);
        if (result != PasswordVerificationResult.Success)
            return Unauthorized("Invalid username or password.");

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token, Role = user.Role });
    }
 
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT is stateless; logout should be handled client-side or with token blacklisting
        return Ok("User logged out (client-side).");
    }
 
    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(Guid userId, string role)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");
 
        user.Role = role;
        await _context.SaveChangesAsync();
 
        return Ok($"Role changed to {role}");
    }
}