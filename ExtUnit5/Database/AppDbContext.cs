using ExtUnit5;
using ExtUnit5.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems);
            });
        }
    }
}
