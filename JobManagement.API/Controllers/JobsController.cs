using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JobManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Job>>> GetAllJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Job>>> GetActiveJobs()
    {
        var jobs = await _jobService.GetActiveJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Job>> GetJob(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpPost]
    public async Task<ActionResult<JobCreationResponse>> CreateJob(JobCreationRequest request)
    {
        try
        {
            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Requirements = request.Requirements,
                SalaryMin = request.SalaryMin,
                SalaryMax = request.SalaryMax,
                Location = request.Location,
                ApplicationDeadline = request.ApplicationDeadline,
                MaxApplications = request.MaxApplications
            };

            var jobId = await _jobService.CreateJobAsync(job, request.CreatorId);

            return Ok(new JobCreationResponse
            {
                JobId = jobId,
                Message = "Job created successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/publish")]
    public async Task<ActionResult> PublishJob(int id, [FromBody] PublishJobRequest request)
    {
        try
        {
            await _jobService.PublishJobAsync(id, request.PublisherId);
            return Ok(new { Message = "Job published successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    public class JobCreationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime ApplicationDeadline { get; set; }
        public int CreatorId { get; set; }
        public int? MaxApplications { get; set; }
    }

    public class JobCreationResponse
    {
        public int JobId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PublishJobRequest
    {
        public int PublisherId { get; set; }
    }
}