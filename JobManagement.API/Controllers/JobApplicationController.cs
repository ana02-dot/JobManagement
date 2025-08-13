using JobManagement.Application.Services;
using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JobManagement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class JobApplicationController : ControllerBase
{
    private readonly JobApplicationService _jobApplicationService;

    public JobApplicationController(JobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }
    [HttpPost]
        public async Task<ActionResult<ApplicationSubmissionResponse>> SubmitApplication(ApplicationSubmissionRequest request)
        {
            try
            {
                var application = new JobApplication
                {
                    JobId = request.JobId,
                    CoverLetter = request.CoverLetter,
                    Resume = request.Resume
                };

                var applicationId = await _jobApplicationService.SubmitApplicationAsync(application, request.ApplicantId);
                
                return Ok(new ApplicationSubmissionResponse
                {
                    ApplicationId = applicationId,
                    Message = "Application submitted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetApplicationsByJob(int jobId)
        {
            var applications = await _jobApplicationService.GetApplicationsByJobAsync(jobId);
            return Ok(applications);
        }

        [HttpGet("applicant/{applicantId}")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetApplicationsByApplicant(int applicantId)
        {
            var applications = await _jobApplicationService.GetApplicationsByApplicantAsync(applicantId);
            return Ok(applications);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetPendingApplications()
        {
            var applications = await _jobApplicationService.GetPendingApplicationsAsync();
            return Ok(applications);
        }

        [HttpPut("{id}/review")]
        public async Task<ActionResult> ReviewApplication(int id, [FromBody] ReviewApplicationRequest request)
        {
            try
            {
                await _jobApplicationService.ReviewApplicationAsync(
                    id, request.ReviewerId, request.Status, request.ReviewNotes);
                
                return Ok(new { Message = "Application reviewed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    public class ApplicationSubmissionRequest
    {
        public int JobId { get; set; }
        public int ApplicantId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public string? Resume { get; set; }
    }

    public class ApplicationSubmissionResponse
    {
        public int ApplicationId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ReviewApplicationRequest
    {
        public int ReviewerId { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? ReviewNotes { get; set; }
    }
}