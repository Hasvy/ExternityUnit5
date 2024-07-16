using Database;
using ExtUnit5.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Database
{
    public class Seeder
    {
        private IDbContextFactory<AppDbContext> _dbContextFactory;
        private FakeDataService _dataService;

        public Seeder(IDbContextFactory<AppDbContext> dbContextFactory, FakeDataService dataService)
        {
            _dbContextFactory = dbContextFactory;
            _dataService = dataService;
        }

        public void SeedDatabase()
        {
            _dataService.Init(10);
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Categories.AddRange(_dataService.CategoryList);
                context.Products.AddRange(_dataService.ProductList);
                context.Customers.AddRange(_dataService.CustomerList);
                context.Orders.AddRange(_dataService.OrderList);
                context.OrderItems.AddRange(_dataService.OrderItemsList);
                context.SaveChanges();
            }
        }
    }
}
