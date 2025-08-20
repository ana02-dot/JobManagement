using System.Security.Claims;
using JobManagement.Application.Dtos;
using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using JobManagement.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;
using RegisterRequest = Microsoft.AspNetCore.Identity.Data.RegisterRequest;

namespace JobManagement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly UserService _userService;

    public AuthController(IJwtService jwtService, UserService userService)
    {
        _jwtService = jwtService;
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        Log.Information("Login attempt for email: {Email}", request.Email);
        
        try
        {
            var (isValid, user) = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
            if (!isValid || user == null)
            {
                Log.Warning("Failed login attempt for email: {Email}", request.Email);
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user);
        
            return Ok(new { Token = token, User = user });
            
            Log.Information("Successful login for user {UserId} with email {Email}", user.Id, request.Email);

            return Ok(new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token,
                Message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during login for email: {Email}", request.Email);
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        return Ok(new { Message = "You are authenticated!", User = User.Identity.Name });
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new { Message = "Admin access granted!" });
    }
}