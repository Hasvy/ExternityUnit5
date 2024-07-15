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

        public readonly FakeDataService _fakeDataService;
        public readonly int _fakeDataCount = 10;
        public AppDbContext(DbContextOptions<AppDbContext> options, FakeDataService fakeDataService) : base(options)
        {
            _fakeDataService = fakeDataService;
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
                    .WithMany();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany();
            });

            //modelBuilder.Entity<Customer>().HasData(_fakeDataService.GetCustomers(_fakeDataCount));
            //modelBuilder.Entity<Order>().HasData(_fakeDataService.GetOrders(_fakeDataCount));
        }
    }
}
