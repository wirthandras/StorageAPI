using Microsoft.EntityFrameworkCore;
using StorageAPI.Model;

namespace StorageAPI.Configuration
{
    public class AppDBContext : DbContext
    {
        public DbSet<Car> Car { get; set; }

        public DbSet<CarPrice> Price { get; set; }

        public DbSet<Image> Image { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarPrice>()
                .HasOne(e => e.Car)
                .WithMany(e => e.Prices)
                .HasForeignKey(e => e.CarId)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .HasOne(e => e.Car)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.CarId)
                .IsRequired();
        }
    }
}
