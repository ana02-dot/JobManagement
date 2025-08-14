using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using JobManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobManagementDbContext _context;

    public JobRepository(JobManagementDbContext context)
    {
        _context = context;
    }
    public async Task<Job?> GetByIdAsync(int id)
    {
        return await _context.Jobs
            .Include(j => j.CreatedBy)
            .Include(j => j.Applications)
            .Where(j => !j.IsDeleted)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(j => j.CreatedBy)
            .Where(j => !j.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status)
    {
        return await _context.Jobs
            .Include(j => j.CreatedBy)
            .Where(j => !j.IsDeleted && j.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetByCreatorAsync(int creatorId)
    {
        return await _context.Jobs
            .Include(j => j.CreatedBy)
            .Where(j => !j.IsDeleted && j.CreatedByUserId == creatorId)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job.Id;
    }

    public async Task UpdateAsync(Job job)
    {
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var job = await GetByIdAsync(id);
        if (job != null)
        {
            job.IsDeleted = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Jobs
            .Where(j => !j.IsDeleted)
            .AnyAsync(j => j.Id == id);
    }
}