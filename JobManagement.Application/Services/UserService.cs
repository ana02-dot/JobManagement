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

        public UserService(
            IUserRepository userRepository, 
            IPhoneValidationService phoneValidationService)
        {
            _userRepository = userRepository;
            _phoneValidationService = phoneValidationService;
        }

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _userRepository.GetByIdAsync(id);
        
        public async Task<User?> GetUserByEmailAsync(string email) =>
             await _userRepository.GetByEmailAsync(email);
        
        public async Task<IEnumerable<User>> GetAllUsersAsync() =>
            await _userRepository.GetAllAsync();

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
            //user.CreatedBy = user.Id;

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
        
        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new ValidationException("User not found");
            }
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return user;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(id);
            return true;
        }public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new ValidationException("New password must be at least 6 characters long");
            }

            // Hash and update new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
        
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new ValidationException("Password must be at least 6 characters long");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
        
            await _userRepository.UpdateAsync(user);
            return true;
        }
}