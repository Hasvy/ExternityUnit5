﻿using Database;
using ExtUnit5.Entities;
using ExtUnit5.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Database
{
    public class Seeder
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly FakeDataService _dataService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public Seeder(IDbContextFactory<AppDbContext> dbContextFactory, FakeDataService dataService, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _dataService = dataService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedDatabase()
        {
            string? userName = _configuration["UserSettings:UserName"];

            if (userName != null)
                await _userManager.CreateAsync(new IdentityUser(userName));

            _dataService.Init();
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Categories.AddRange(_dataService.CategoryList);
                context.Products.AddRange(_dataService.ProductList);
                context.Customers.AddRange(_dataService.CustomerList);
                context.Orders.AddRange(_dataService.OrderList);
                context.OrderItems.AddRange(_dataService.OrderItemsList);
                context.SaveChanges();
            }

            using (var context = _dbContextFactory.CreateDbContext())
            {
                ActualizeCustomersGroups(context);
                ActualizeProducts(context);
                context.SaveChanges();
            }
        }

        private void ActualizeCustomersGroups(AppDbContext context)     //This process should be performed after order is finished, but we don't have this feature yet
        {
            Dictionary<Customer, decimal> customersDict = new Dictionary<Customer, decimal>();
            foreach (var customer in context.Customers.ToList())
            {
                if (customer.Orders is not null)
                {
                    customersDict.Add(customer, customer.Orders.Where(o => o.Status == OrderStatus.Finished).Sum(o => o.TotalAmount));
                }
            }
            var sortedCustomersDict = customersDict.OrderByDescending(c => c.Value);
            var indexOftreshold = (int)(0.10 * sortedCustomersDict.Count());
            var treshold = sortedCustomersDict.ToArray()[indexOftreshold].Value;
            foreach (var customer in context.Customers.ToList())
            {
                if (customer.Orders is not null)
                {
                    if (customer.Orders.Where(o => o.Status == OrderStatus.Finished).Sum(o => o.TotalAmount) > treshold)
                    {
                        customer.CustomerGroup = CustomerGroup.VIP;
                        continue;
                    }

                    bool boughtInLastMonth = customer.Orders.ToList().Exists(o => o.OrderDate.Month == DateTime.Now.AddMonths(-1).Month);
                    bool boughtInPreLastMonth = customer.Orders.ToList().Exists(o => o.OrderDate.Month == DateTime.Now.AddMonths(-2).Month);

                    if (boughtInLastMonth && boughtInPreLastMonth)
                    {
                        customer.CustomerGroup = CustomerGroup.Regular;
                        continue;
                    }

                    if (customer.RegistrationDate > DateTime.Today.AddMonths(-1))
                        customer.CustomerGroup = CustomerGroup.New;
                }
            }
        }

        private void ActualizeProducts(AppDbContext context)        //Also would be performed after order is finished
        {
            foreach (var product in context.Products.ToList())
            {
                var productOrders = context.OrderItems.Where(oi => oi.Product == product);
                int totalOrders = productOrders.Count();
                DateTime firstMonthProductSold = productOrders.OrderBy(o => o.Order.OrderDate).First().Order.OrderDate;
                int productAvailableInMonths = ((DateTime.Now.Year - firstMonthProductSold.Year) * 12) + DateTime.Now.Month - firstMonthProductSold.Month;

                product.AverageOrdered = (float)totalOrders / productAvailableInMonths;

                int orderedThisMonth = productOrders.Where(
                    oi => oi.Product == product &&
                    oi.Order.OrderDate.Year == DateTime.Now.Year &&
                    oi.Order.OrderDate.Month == DateTime.Now.Month)
                    .Count();

                float shouldBeOrderedPerDay = product.AverageOrdered / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                product.Popularity = orderedThisMonth / (shouldBeOrderedPerDay * DateTime.Now.Day);
            }
        }
    }
}
