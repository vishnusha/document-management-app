using DocumentManagement.Domain.Entities;
using DocumentManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Infrastructure.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

            await context.Database.MigrateAsync();

            // Seed Users
            if (!context.Users.Any())
            {
                var roles = new[] { "Admin", "Editor", "Viewer" };
                var users = new List<User>();

                for (int i = 1; i <= 1000; i++)
                {
                    var role = roles[i % 3];
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = $"user{i}@example.com",
                        Email = $"user{i}@example.com",
                        Role = role
                    };
                    user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");
                    users.Add(user);
                }

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // Seed Documents
            if (!context.Documents.Any())
            {
                var users = await context.Users.Take(1000).ToListAsync();
                var documents = new List<Document>();

                for (int i = 1; i <= 100000; i++)
                {
                    var doc = new Document
                    {
                        Id = Guid.NewGuid(),
                        Title = $"Document {i}",
                        Content = $"This is the content of document {i}.",
                        CreatedByUserId = users[i % users.Count].Id,
                        UploadedAt = DateTime.UtcNow
                    };
                    documents.Add(doc);
                }

                await context.Documents.AddRangeAsync(documents);
                await context.SaveChangesAsync();
            }
        }
    }
}
