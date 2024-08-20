using Database;
using ExtUnit5.Entities;
using ExtUnit5.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ExtUnit5.Jobs
{
    public class AdjustProductsPricesJob : IJob
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly CodeGeneratorService _codeGeneratorService;

        private AppDbContext dbContext { get; set; } = null!;
        private const float priceChangeProportion = 0.25f;
        private const int couponExpirationDays = 7;

        public AdjustProductsPricesJob(IDbContextFactory<AppDbContext> dbContextFactory, CodeGeneratorService codeGeneratorService)
        {
            _dbContextFactory = dbContextFactory;
            _codeGeneratorService = codeGeneratorService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            dbContext = _dbContextFactory.CreateDbContext();

            foreach (var product in dbContext.Products.ToList())
            {
                //Adds 1/4 of popularity to price (price 100, popularity 1.40 => price will be 110)
                if (product.PopularityGroup is Product.Group.Popular && product.Popularity != product.LastPopularityAffectedPrice)
                {
                    product.Price += (product.Popularity - 1) * priceChangeProportion * product.Price;
                    product.Price = (float)Math.Round(product.Price, 2);
                    product.LastPopularityAffectedPrice = product.Popularity;
                    //Some actions only for popular products
                }

                if (product.PopularityGroup is Product.Group.Unpopular && product.Popularity != product.LastPopularityAffectedPrice)
                {
                    if (product.Popularity < 0.5f)      //If product is very unpopular creates coupons for the customers
                    {
                        var customers = dbContext.Customers.Where(c => c.CustomerGroup != CustomerGroup.Basic).ToList();
                        GenerateCoupons(product, customers);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        product.Price += (product.Popularity - 1) * priceChangeProportion * product.Price;
                        product.Price = (float)Math.Round(product.Price, 2);
                    }
                    product.LastPopularityAffectedPrice = product.Popularity;
                    //Porbably a notification that product is cheaper now or something only for the unpopular products
                }
            }
            dbContext.SaveChanges();

            return Task.CompletedTask;
        }

        private void GenerateCoupons(Product product, List<Customer> customers)
        {    
            foreach (var customer in customers)
            {
                Coupon coupon = new Coupon(_codeGeneratorService.GenerateCode(12),
                                           customer,
                                           product,
                                           DateTime.Now.AddDays(couponExpirationDays).Date);
                dbContext.Coupons.Add(coupon);
            }
        }
    }
}
