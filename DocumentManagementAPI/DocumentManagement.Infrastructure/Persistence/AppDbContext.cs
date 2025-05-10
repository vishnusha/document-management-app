using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Persistence
{
   public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { 
            
        }
 
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }

}
}