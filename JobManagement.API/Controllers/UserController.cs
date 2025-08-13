using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JobManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        user.PasswordHash = string.Empty;
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserRegistrationRequest>> RegisterUser(UserRegistrationRequest request)
    {
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
            return Ok(new UserRegistrationResponse
            {
                UserId = userId,
                Message = "User created successfully",
                IsPersonalNumberVerified = true
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        try
        {
            var isValid = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
            if (!isValid)
                return Unauthorized(new { Message = "Invalid credentials" });

            var user = await _userService.GetUserByEmailAsync(request.Email);

            return Ok(new LoginResponse
            {
                UserId = user!.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Message = "Login successful"
            });
        }
        catch (Exception ex)
        {
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
        public bool IsPersonalNumberVerified { get; set; }
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
        public string Message { get; set; } = string.Empty;
    }
}