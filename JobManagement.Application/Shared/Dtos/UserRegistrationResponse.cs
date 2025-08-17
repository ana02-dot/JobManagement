namespace JobManagement.Application.Dtos;

public class UserRegistrationResponse
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsPhoneNumberVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}