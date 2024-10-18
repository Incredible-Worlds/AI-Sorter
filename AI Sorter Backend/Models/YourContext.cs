using Microsoft.EntityFrameworkCore;

namespace AI_Sorter_Backend.Models
{
    public class YourContext : DbContext
    {
        public YourContext(DbContextOptions<YourContext> options) : base(options)
        {
        }

        // File Table
        public DbSet<FIleEntity> Files { get; set; }
    }
}