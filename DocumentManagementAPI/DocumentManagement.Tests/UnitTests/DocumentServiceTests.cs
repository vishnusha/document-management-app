using Microsoft.EntityFrameworkCore;
using Xunit;
using DocumentManagement.Application.Services;
using DocumentManagement.Infrastructure.Persistence;
using DocumentManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DocumentServiceTests
{
    private readonly DocumentService _documentService;
    private readonly AppDbContext _dbContext;

    public DocumentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDb")
                        .Options;

        _dbContext = new AppDbContext(options);
        _documentService = new DocumentService(_dbContext);
    }

    [Fact]

    public async Task GetAll_ShouldReturnDocuments()
    {
        // Arrange
        var mockDocuments = new List<Document>
    {
        new Document { Id = Guid.NewGuid(), Title = "Document 1", Content = "Content 1" },
        new Document { Id = Guid.NewGuid(), Title = "Document 2", Content = "Content 2" }
    };

        await _dbContext.Documents.AddRangeAsync(mockDocuments);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _documentService.GetAllAsync();

        // Assert
        Assert.NotEmpty(result); // Ensure it's not empty
        Assert.Equal(2, result.Count); // Ensure it returns the correct number of documents
    }


    [Fact]
    public async Task GetAllPagedAsync_ShouldReturnPagedDocuments()
    {
        // Arrange
        var mockDocuments = new List<Document>
        {
            new Document { Id = Guid.NewGuid(), Title = "Document 1", Content = "Content 1" },
            new Document { Id = Guid.NewGuid(), Title = "Document 2", Content = "Content 2" },
            new Document { Id = Guid.NewGuid(), Title = "Document 3", Content = "Content 3" },
            new Document { Id = Guid.NewGuid(), Title = "Document 4", Content = "Content 4" }
        };

        await _dbContext.Documents.AddRangeAsync(mockDocuments);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _documentService.GetAllPagedAsync(1, 2); // Page 1, PageSize 2

        // Assert
        Assert.Equal(2, result.Count); // Page 1 with size 2 should return 2 documents
        Assert.Equal("Document 1", result[0].Title); // Ensure first document is "Document 1"
        Assert.Equal("Document 2", result[1].Title); // Ensure second document is "Document 2"
    }
}
