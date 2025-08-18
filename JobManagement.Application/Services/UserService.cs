using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;
using BCrypt.Net;


namespace JobManagement.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPhoneValidationService _phoneValidationService;
    private readonly IJwtService _jwtService;

        public UserService(
            IUserRepository userRepository, 
            IPhoneValidationService phoneValidationService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _phoneValidationService = phoneValidationService;
            _jwtService = jwtService;
        }

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _userRepository.GetByIdAsync(id);
        
        public async Task<User?> GetUserByEmailAsync(string email) =>
             await _userRepository.GetByEmailAsync(email);
        

        public async Task<User> CreateUserAsync(User user, string password)
        {
            // Validate email uniqueness
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new ValidationException("Email already exists");
            }

            // Validate phone number
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var phoneValidationResult = await _phoneValidationService.ValidatePhoneAsync(user.PhoneNumber);
                if (!phoneValidationResult.IsValid)
                {
                    throw new ValidationException($"Invalid phone number: {phoneValidationResult.ErrorMessage}");
                }
            }

            // Validate phone number uniqueness
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await _userRepository.PhoneNumberExistsAsync(user.PhoneNumber))
            {
                throw new ValidationException("Phone number already exists");
            }
            
            // Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            user.CreatedBy = user.Id;

            var userId =  await _userRepository.CreateAsync(user);
            user.Id = userId; // Set the ID from the repository
            return user;
        }

        public async Task<(bool IsValid, User? User)> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return (false, null);

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return (isValid, isValid ? user : null);
        }

        public string GenerateJwtToken(User user) =>
            _jwtService.GenerateToken(user);

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
}