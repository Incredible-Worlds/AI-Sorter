using Microsoft.EntityFrameworkCore;
namespace AI_Sorter_Backend.Models
{
    public class DbContext
    {
        public class ApplicationDbContext : DbContext
        {
        
            public DbSet<FIleEntity> postgres { get; set; }

            public void OnModelCreating(ModelBuilder modelBuilder)
            {
             
                modelBuilder.Entity<FIleEntity>()
                    .HasKey(f => f.id); // Установка первичного ключа

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.file_name)
                    .IsRequired(); // Обязательное поле для имени файла

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.path_file_itg)
                    .IsRequired(); // Обязательное поле для пути итерационного файла

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.promt_sort)
                    .IsRequired(false); // Необязательное поле для сортировки

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.path_file)
                    .IsRequired(false); // Необязательное поле для пути файла

                modelBuilder.Entity<FIleEntity>()
                    .Property(f => f.Status_sort)
                    .IsRequired(false); // Необязательное поле для статуса сортировки
            }
        }
    }
}
