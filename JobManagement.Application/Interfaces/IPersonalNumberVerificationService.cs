using JobManagement.Application.Services;

namespace JobManagement.Application.Interfaces;

public interface IPersonalNumberVerificationService
{
    Task<PersonalNumberVerificationResult> VerifyPersonalNumberAsync(string personalNumber);
}