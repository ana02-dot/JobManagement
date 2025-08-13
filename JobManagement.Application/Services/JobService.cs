using JobManagement.Application.Interfaces;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;

namespace JobManagement.Application.Services;

public class JobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IUserRepository _userRepository;
    
    public JobService(IJobRepository jobRepository, IUserRepository userRepository)
    {
        _jobRepository = jobRepository;
        _userRepository = userRepository;
    }
     public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _jobRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _jobRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _jobRepository.GetByStatusAsync(JobStatus.Active);
        }

        public async Task<int> CreateJobAsync(Job job, int creatorId)
        {
            // Validate creator exists and has appropriate role
            var creator = await _userRepository.GetByIdAsync(creatorId);
            if (creator == null)
                throw new InvalidOperationException("Creator not found");

            if (creator.Role != UserRole.HR && creator.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Only HR and Admin users can create jobs");

            job.CreatedByUserId = creatorId;
            job.CreatedAt = DateTime.UtcNow;
            job.Status = JobStatus.Draft;

            return await _jobRepository.CreateAsync(job);
        }

        public async Task UpdateJobAsync(Job job, int updaterId)
        {
            var existingJob = await _jobRepository.GetByIdAsync(job.Id);
            if (existingJob == null)
                throw new InvalidOperationException("Job not found");

            var updater = await _userRepository.GetByIdAsync(updaterId);
            if (updater == null)
                throw new InvalidOperationException("Updater not found");

            // Only creator, HR, or Admin can update
            if (existingJob.CreatedByUserId != updaterId && 
                updater.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Insufficient permissions to update job");

            job.UpdatedAt = DateTime.UtcNow;
            job.UpdatedBy = updater.Email;

            await _jobRepository.UpdateAsync(job);
        }

        public async Task PublishJobAsync(int jobId, int publisherId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
                throw new InvalidOperationException("Job not found");

            var publisher = await _userRepository.GetByIdAsync(publisherId);
            if (publisher == null)
                throw new InvalidOperationException("Publisher not found");

            if (job.CreatedByUserId != publisherId && publisher.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("Insufficient permissions to publish job");

            job.Status = JobStatus.Active;
            job.UpdatedAt = DateTime.UtcNow;
            job.UpdatedBy = publisher.Email;

            await _jobRepository.UpdateAsync(job);
        }
}