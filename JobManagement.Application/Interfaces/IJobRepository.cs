using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;

namespace JobManagement.Application.Interfaces;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(int id);
    Task<IEnumerable<Job>> GetAllAsync();
    Task<IEnumerable<Job>> GetByStatusAsync(JobStatus status);
    Task<IEnumerable<Job>> GetByCreatorAsync(int creatorId);
    Task<int> CreateAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}