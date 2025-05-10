// Application/DTOs/LoginRequest.cs
namespace DocumentManagement.Application.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}