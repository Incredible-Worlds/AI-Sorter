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

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				if (!optionsBuilder.IsConfigured)
				{
					// Укажите строку подключения к вашей базе данных PostgreSQL
					optionsBuilder.UseNpgsql("Host=db; Database=postgres; Username=postgres; Password=BlazorApp");
				}
			}

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				modelBuilder.Entity<FIleEntity>()
					.HasKey(f => f.id);

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.file_name)
					.IsRequired(false);

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.unic_file_name)
					.IsRequired();

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.path_file)
					.IsRequired(false);

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.path_file_competed)
					.IsRequired();

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.datetime)
					.IsRequired();

				modelBuilder.Entity<FIleEntity>()
					.Property(f => f.Status_sort)
					.IsRequired(false);
			}
		}
    }
}
