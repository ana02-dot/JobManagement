namespace JobManagement.Application.Services;

public class PersonalNumberVerificationResult
{
    public bool IsValid { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ErrorMessage { get; set; }

}