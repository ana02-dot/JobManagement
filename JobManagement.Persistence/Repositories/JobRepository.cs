using JobManagement.Application.Dtos;
using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using JobManagement.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Persistence.Repositories;

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
            .Include(j => j.Creator)
            .Include(j => j.Applications)
            .Where(j => !j.IsDeleted)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<List<Job>> GetAllAsync()
    {
        return await _context.Jobs
            .Include(j => j.Creator)
            .Where(j => !j.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Job>> GetByStatusAsync(JobStatus status)
    {
        return await _context.Jobs
            .Where(j => j.Status == status && !j.IsDeleted) 
            .ToListAsync();
    }

    public async Task<Job> CreateAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
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
}