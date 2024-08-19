using Database;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExtUnit5.Jobs
{
    public class AdjustProductsPricesJob : IJob
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private AppDbContext dbContext { get; set; } = null!;

        public AdjustProductsPricesJob(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Task Execute(IJobExecutionContext context)       //Also would be performed after order is finished
        {
            dbContext = _dbContextFactory.CreateDbContext();

            foreach (var product in dbContext.Products.ToList())
            {
                int orderedThisMonth = dbContext.OrderItems.Where(
                    oi => oi.Product == product &&
                    oi.Order.OrderDate.Year == DateTime.Now.Year &&
                    oi.Order.OrderDate.Month == DateTime.Now.Month)
                    .Count();

                float shouldBeOrderedPerDay = product.AverageOrdered / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                product.Popularity = orderedThisMonth / (shouldBeOrderedPerDay * DateTime.Now.Day);

                //TODO adjust prices if popular => + 10%, if unpopular => -10%
            }
            dbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
