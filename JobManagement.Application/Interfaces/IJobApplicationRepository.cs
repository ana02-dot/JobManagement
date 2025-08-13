using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;

namespace JobManagement.Application.Interfaces;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(int id);
    Task<IEnumerable<JobApplication>> GetByJobIdAsync(int jobId);
    Task<IEnumerable<JobApplication>> GetByApplicantIdAsync(int applicantId);
    Task<IEnumerable<JobApplication>> GetByStatusAsync(ApplicationStatus status);
    Task<int> CreateAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
    Task DeleteAsync(int id);
    Task<bool> HasUserAppliedAsync(int jobId, int userId);
}