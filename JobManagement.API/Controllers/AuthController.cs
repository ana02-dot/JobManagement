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
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] UserRegistrationRequest request)
    {
        try
        {
            Log.Information("User registration attempt for email: {Email}", request.Email);

            var user = new User
            {
                PersonalNumber = request.PersonalNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                IsPersonalNumberVerified = false,
                IsEmailVerified = false
            };

            var createdUser = await _userService.CreateUserAsync(user, request.Password);
            var token = _userService.GenerateJwtToken(createdUser);
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            Log.Information("User registered successfully: {Email}", request.Email);

            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserInfo
                {
                    Id = createdUser.Id,
                    PersonalNumber = createdUser.PersonalNumber,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Email = createdUser.Email,
                    PhoneNumber = createdUser.PhoneNumber,
                    Role = createdUser.Role,
                    IsPersonalNumberVerified = createdUser.IsPersonalNumberVerified,
                    IsEmailVerified = createdUser.IsEmailVerified,
                    CreatedAt = createdUser.CreatedAt
                }
            });
        }
        catch (ValidationException ex)
        {
            Log.Warning("Registration validation failed for {Email}: {Message}", request.Email, ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error during registration for {Email}", request.Email);
            return StatusCode(500, new { Message = "Registration failed. Please try again." });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            Log.Information("Login attempt for email: {Email}", request.Email);

            var (isValid, user) = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
            
            if (!isValid || user == null)
            {
                Log.Warning("Invalid login attempt for email: {Email}", request.Email);
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            var token = _userService.GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            Log.Information("User logged in successfully: {Email}", request.Email);

            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserInfo
                {
                    Id = user.Id,
                    PersonalNumber = user.PersonalNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    IsPersonalNumberVerified = user.IsPersonalNumberVerified,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error during login for {Email}", request.Email);
            return StatusCode(500, new { Message = "Login failed. Please try again." });
        }
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfo), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<UserInfo>> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { Message = "Invalid token" });

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(new UserInfo
            {
                Id = user.Id,
                PersonalNumber = user.PersonalNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsPersonalNumberVerified = user.IsPersonalNumberVerified,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting user profile");
            return StatusCode(500, new { Message = "Failed to retrieve profile" });
        }
    }
}