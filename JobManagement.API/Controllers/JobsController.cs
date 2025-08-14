using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace JobManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// Get all jobs (requires Admin or Manager role)
    /// </summary>
    /// <returns>List of all jobs</returns>
    /// <response code="200">Returns the list of all jobs</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have required role</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(List<Job>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<List<Job>>> GetAllJobs()
    {
        Log.Information("Getting all jobs");
        var jobs = await _jobService.GetAllJobsAsync();
        Log.Information("Retrieved {JobCount} jobs", jobs.Count());
        return Ok(jobs);
    }

    /// <summary>
    /// Get all active/published jobs (public endpoint)
    /// </summary>
    /// <returns>List of active jobs</returns>
    /// <response code="200">Returns the list of active jobs</response>
    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<Job>), 200)]
    public async Task<ActionResult<List<Job>>> GetActiveJobs()
    {
        Log.Information("Getting active jobs");
        var jobs = await _jobService.GetActiveJobsAsync();
        Log.Information("Retrieved {JobCount} active jobs", jobs.Count());
        return Ok(jobs);
    }

    /// <summary>
    /// Get a specific job by ID
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>Job information</returns>
    /// <response code="200">Returns the job information</response>
    /// <response code="404">If the job is not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have required role</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(Job), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<Job>> GetJob(int id)
    {
        Log.Information("Getting job with ID: {JobId}", id);
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            Log.Warning("Job with ID {JobId} not found", id);
            return NotFound();
        }

        Log.Information("Successfully retrieved job {JobId}: {JobTitle}", id, job.Title);
        return Ok(job);
    }

    /// <summary>
    /// Create a new job posting
    /// </summary>
    /// <param name="request">Job creation data</param>
    /// <returns>Job creation result</returns>
    /// <response code="200">Job created successfully</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have required role</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(JobCreationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<JobCreationResponse>> CreateJob([FromBody] JobCreationRequest request)
    {
        Log.Information("Creating new job with title: {JobTitle}", request.Title);
        
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

            Log.Information("Successfully created job {JobId} with title {JobTitle}", jobId, request.Title);

            return Ok(new JobCreationResponse
            {
                JobId = jobId,
                Message = "Job created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating job with title: {JobTitle}", request.Title);
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Publish a job to make it visible to applicants
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <param name="request">Publishing data</param>
    /// <returns>Publishing result</returns>
    /// <response code="200">Job published successfully</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="404">If the job is not found</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have required role</response>
    [HttpPut("{id}/publish")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> PublishJob(int id, [FromBody] PublishJobRequest request)
    {
        Log.Information("Publishing job {JobId} by publisher {PublisherId}", id, request.PublisherId);
        
        try
        {
            await _jobService.PublishJobAsync(id, request.PublisherId);
            Log.Information("Successfully published job {JobId}", id);
            return Ok(new { Message = "Job published successfully" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error publishing job {JobId}", id);
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