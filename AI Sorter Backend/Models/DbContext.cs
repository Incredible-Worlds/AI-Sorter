using Microsoft.EntityFrameworkCore;
namespace AI_Sorter_Backend.Models
{
    public class DbContex
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }
        
            public DbSet<FIleEntity> BlazorApp { get; set; }

            public void OnModelCreating(ModelBuilder modelBuilder)
            {
             
                modelBuilder.Entity<FIleEntity>()
                    .HasKey(f => f.id); 

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.file_name)
                    .IsRequired(); 

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.path_file_itg)
                    .IsRequired(); 

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.promt_sort)
                    .IsRequired(false); 

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.path_file)
                    .IsRequired(false); 

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.Status_sort)
                    .IsRequired(false); 
            }
        }
    }
}
