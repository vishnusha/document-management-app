using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagement.API.DTOs
{
    public class UploadDocumentRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;

        [Required]
        public string Title { get; set; } = default!;
    }
}
