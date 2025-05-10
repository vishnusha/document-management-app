// Application/DTOs/RegisterRequest.cs
namespace DocumentManagement.Application.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }  // Add Email property
    }
}
