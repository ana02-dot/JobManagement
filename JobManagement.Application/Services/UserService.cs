using System.Security.Cryptography;
using System.Text;
using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;

namespace JobManagement.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonalNumberVerificationService _personalNumberVerificationService;

        public UserService(
            IUserRepository userRepository,
            IPersonalNumberVerificationService personalNumberVerificationService)
        {
            _userRepository = userRepository;
            _personalNumberVerificationService = personalNumberVerificationService;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<int> CreateUserAsync(User user, string password)
        {
            // Validate email uniqueness
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Validate personal number uniqueness
            if (await _userRepository.PersonalNumberExistsAsync(user.PersonalNumber))
            {
                throw new InvalidOperationException("Personal number already exists");
            }

            // Verify personal number with Georgian service
            var verificationResult = await _personalNumberVerificationService
                .VerifyPersonalNumberAsync(user.PersonalNumber);

            if (!verificationResult.IsValid)
            {
                throw new InvalidOperationException($"Invalid personal number: {verificationResult.ErrorMessage}");
            }

            // Set verified information from Georgian service
            user.FirstName = verificationResult.FirstName ?? user.FirstName;
            user.LastName = verificationResult.LastName ?? user.LastName;
            user.IsPersonalNumberVerified = true;
            user.PersonalNumberVerifiedAt = DateTime.UtcNow;

            // Hash password
            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = password + "JobManagementSalt2024"; // In production, use a proper salt
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashToVerify = HashPassword(password);
            return hashToVerify == storedHash;
        }
}