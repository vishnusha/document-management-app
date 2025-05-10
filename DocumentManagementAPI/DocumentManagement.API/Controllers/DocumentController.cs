using DocumentManagement.API.DTOs;
using DocumentManagement.Application.DTOs;
using DocumentManagement.Application.Services;
using DocumentManagement.Domain.Entities;
using DocumentManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace DocumentManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _docService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public DocumentController(DocumentService docService, AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _docService = docService;
            _env = env;
        }
        

        // VIEW ALL - Admin, Editor, Viewer
        [Authorize(Roles = "Admin,Editor,Viewer")]
        // public async Task<IActionResult> GetAll() =>
        //     Ok(await _docService.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var documents = await _docService.GetAllPagedAsync(pageNumber, pageSize);
            return Ok(documents);
        }

        // VIEW SINGLE - Admin, Editor, Viewer
        [Authorize(Roles = "Admin,Editor,Viewer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var doc = await _docService.GetByIdAsync(id);
            return doc == null ? NotFound() : Ok(doc);
        }

        // CREATE - Admin, Editor
        [Authorize(Roles = "Admin,Editor")]
        [HttpPost]
        public async Task<IActionResult> Create(DocumentDto dto)
        {
            var doc = new Document
            {
                Title = dto.Title,
                Content = dto.Content,
                UploadedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown"
            };

            return Ok(await _docService.CreateAsync(doc));
        }

        // UPDATE - Admin, Editor
        [Authorize(Roles = "Admin,Editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, DocumentDto dto)
        {
            var updated = new Document
            {
                Id = id,
                Title = dto.Title,
                Content = dto.Content
            };

            var result = await _docService.UpdateAsync(updated);
            return result ? Ok() : NotFound();
        }

        
        
        [HttpGet("download/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadDocument(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound();

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "UploadedDocuments");
            var filePath = Path.Combine(uploadsFolder, document.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            // Get the file extension to determine the content type dynamically
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
            var originalFileName = document.FilePath.Substring(document.FilePath.IndexOf('_') + 1);


            return File(System.IO.File.OpenRead(filePath), contentType, originalFileName);

        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "UploadedDocuments");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.File.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var document = new Document
            {
                Title = request.Title,
                FilePath = fileName,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(document);
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null)
                return NotFound();

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
