using Microsoft.EntityFrameworkCore;
using SecureFileUploadDemo.Models;

namespace SecureFileUploadDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileRecord> FileRecords { get; set; } = null!;
    }
}
