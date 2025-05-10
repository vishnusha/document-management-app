
using DocumentManagement.Infrastructure.Persistence;
using DocumentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace DocumentManagement.Application.Services
{
    public class DocumentService
    {
        private readonly AppDbContext _context;
        private readonly string _uploadPath;

        public DocumentService(AppDbContext context)
        {
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedDocuments");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<List<Document>> GetAllAsync()
        {
            return await _context.Documents
                                 .AsNoTracking()
                                 .ToListAsync();
        }
        public async Task<List<Document>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Documents
                          .AsNoTracking()
                          .OrderByDescending(d => d.UploadedAt) // Replace with your actual date field
                          .Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
        }


        public async Task<Document?> GetByIdAsync(Guid id) =>
            await _context.Documents.FindAsync(id);

        public async Task<Document> CreateAsync(Document doc)
        {
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<bool> UpdateAsync(Document updatedDoc)
        {
            var existing = await _context.Documents.FindAsync(updatedDoc.Id);
            if (existing == null) return false;

            existing.Title = updatedDoc.Title;
            existing.Content = updatedDoc.Content;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null) return false;

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
