﻿using Database;
using ExtUnit5.Entities;
using ExtUnit5.Services;
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
            _dataService.Init();
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Categories.AddRange(_dataService.CategoryList);
                context.Products.AddRange(_dataService.ProductList);
                context.Customers.AddRange(_dataService.CustomerList);
                context.Orders.AddRange(_dataService.OrderList);
                context.OrderItems.AddRange(_dataService.OrderItemsList);
                context.SaveChanges();
                ActualizeCustomersGroups(context);
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
    }
}
