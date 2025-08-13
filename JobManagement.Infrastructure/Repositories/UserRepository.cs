using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;
using JobManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly JobManagementDbContext _context;

    public UserRepository(JobManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPersonalNumberAsync(string personalNumber)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.PersonalNumber == personalNumber);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> PersonalNumberExistsAsync(string personalNumber)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .AnyAsync(u => u.PersonalNumber == personalNumber);
    }
}