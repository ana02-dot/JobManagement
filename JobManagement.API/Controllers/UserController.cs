using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using JobManagement.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace JobManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    /// <response code="200">Returns the user information</response>
    /// <response code="404">If the user is not found</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        Log.Information("Getting user with ID: {UserId}", id);
        
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            Log.Warning("User with ID {UserId} not found", id);
            return NotFound();
        }
        
        user.PasswordHash = string.Empty;
        Log.Information("Successfully retrieved user {UserId}: {UserEmail}", id, user.Email);
        return Ok(user);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <returns>Registration result</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserRegistrationResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UserRegistrationResponse>> RegisterUser([FromBody] UserRegistrationRequest request)
    {
        Log.Information("Registering new user with email: {Email}", request.Email);
        
        try
        {
            var user = new User
            {
                PersonalNumber = request.PersonalNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };

            var userId = await _userService.CreateUserAsync(user, request.Password);
            
            Log.Information("Successfully registered user {UserId} with email {Email}", userId, request.Email);
            
            return Ok(new UserRegistrationResponse
            {
                UserId = userId,
                Message = "User created successfully",
                IsPhoneNumberVerified = true
            });
        }
        catch (JobManagement.Infrastructure.Exceptions.ValidationException e)
        {
            Log.Warning("Validation error during user registration for email {Email}: {ErrorMessage}", request.Email, e.Message);
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error during user registration for email {Email}", request.Email);
            return StatusCode(500, new { Message = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication result with JWT token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">If credentials are invalid</response>
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

            var token = _userService.GenerateJwtToken(user);

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

    public class UserRegistrationRequest
    {
        public string PersonalNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    public class UserRegistrationResponse
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsPhoneNumberVerified { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}