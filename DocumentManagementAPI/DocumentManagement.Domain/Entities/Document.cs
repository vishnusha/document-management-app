namespace DocumentManagement.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Content { get; set; } // Could be actual text or a file path
        public string? UploadedBy { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string? FilePath { get; set; } // Store actual path on disk
    }
}
