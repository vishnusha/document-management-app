using Microsoft.AspNetCore.Http;

namespace DocumentManagement.Application.DTOs
{
    public class DocumentDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        // public IFormFile File { get; set; }
    }
}
