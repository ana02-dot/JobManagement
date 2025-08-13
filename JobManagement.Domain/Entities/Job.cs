using JobManagement.Domain.Common;
using JobManagement.Domain.Enums;

namespace JobManagement.Domain.Entities;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Location { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public int CreatedByUserId { get; set; }
    public int? MaxApplications { get; set; }
        
    // Navigation properties
    public virtual User CreatedBy { get; set; } = null!;
    public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}