using Bogus;
using Database;
using ExtUnit5.Components.Pages.Orders;
using ExtUnit5.Entities;
using ExtUnit5.Entities.Grouping;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Plotly.NET.CSharp;
using Plotly.NET.LayoutObjects;
using Plotly.NET.TraceObjects;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.JSInterop;

namespace ExtUnit5.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; }
        private List<Order> Orders { get; set; }

        private AppDbContext AppDbContext { get; set; } = null!;
        private MarkupString OrdersChart;
        private MarkupString MetricsTable;
        private float meanOrdersValue;
        private float meanOrdersAmount;
        private int totalOrdersCount;
        private List<GroupedProduct> groupedProducts;
        private List<string> popularProducts = new List<string>();
        private List<string> neutralProducts = new List<string>();
        private List<string> unpopularProducts = new List<string>();
        private bool _isLoading;
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            AppDbContext = await DbContextFactory.CreateDbContextAsync();
            Orders = AppDbContext.Orders.ToList();
            OrdersChart = GetOrdersChart();
            totalOrdersCount = GetTotalOrdersCount();
            meanOrdersAmount = GetMeanOrdersAmount();
            ActualizeCustomersGroups();
            //JS.InvokeVoidAsync("renderOrdersChart");
            //MetricsTable = GetMetricsTable();
            await base.OnInitializedAsync();
            _isLoading = false;
        }

        private MarkupString GetOrdersChart()
        {
            #region Dictionary
            //var dict = new Dictionary<DateTime, List<Order>>();
            //foreach (var order in orders)
            //{
            //    DateTime date = order.OrderDate.Date;

            //    if (dict.ContainsKey(date))
            //    {
            //        dict[date].Add(order);
            //    }
            //    else
            //    {
            //        dict.Add(date, new List<Order> { order });
            //    }
            //}
            #endregion

            var groupedOrders = Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    OrderDate = g.Key,
                    Orders = g.ToList()
                })
                .OrderBy(g => g.OrderDate)
                .ToList();

            var a = groupedOrders.Sum(g => g.Orders.Count);
            var b = groupedOrders.Select(g => g.OrderDate).Count();
            meanOrdersValue = (float)a / b;

            var monthlyGroupedOrders = Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(dg => new MonthlyOrder
                    {
                        Month = new DateTime(dg.Key.Year, dg.Key.Month, 1),
                        OrderCount = dg.Count()
                    })
                .OrderBy(o => o.Month)
                .ToList();

            var genericChart = Plotly.NET.CSharp.Chart.Line<DateTime, int, string>(
                y: monthlyGroupedOrders.Select(g => g.OrderCount),
                x: monthlyGroupedOrders.Select(g => g.Month)
                )
                .WithSize(800, 400)
                .WithTraceInfo("Orders Count", ShowLegend: true)
                .WithXAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Month"))
                .WithYAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Orders Count")
                );

            return (MarkupString)Plotly.NET.GenericChart.toChartHTML(genericChart);
        }

        private int GetNewCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.RegistrationDate.Month == DateTime.Today.Month).Count();
        }

        private MarkupString GetProductTrendsChart()
        {
            var orderItems = AppDbContext.OrderItems.ToList();
            groupedProducts = orderItems
                .GroupBy(oi => oi.Product.Id)
                .Select(g => new GroupedProduct
                {
                    ProductId = g.Key,
                    ProductName = AppDbContext.Products.Find(g.Key)?.Name,
                    DatesOrdered = g
                        .GroupBy(oi => new { oi.Order.OrderDate.Year, oi.Order.OrderDate.Month })
                        .Select(dg => new MonthlyOrder
                        {
                            Month = new DateTime(dg.Key.Year, dg.Key.Month, 1),
                            OrderCount = dg.Count()
                        })
                        .OrderBy(d => d.Month)
                        .ToList()
                })
                .OrderByDescending(g => g.DatesOrdered.Count())
                .ToList();

            List<Plotly.NET.GenericChart> charts = new List<Plotly.NET.GenericChart>();

            foreach (var productData in groupedProducts)
            {
                charts.Add(Chart.Line<DateTime, int, string>(
                    x: productData.DatesOrdered.Select(d => d.Month),
                    y: productData.DatesOrdered.Select(d => d.OrderCount),
                    Name: productData.ProductName ?? "Product " + productData.ProductId.ToString()
                    ));
            }

            var combinedChart = Chart.Combine(charts)
                .WithSize(1000)
                .WithXAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Date"))
                .WithYAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Order Count"));

            SeparateProducts();

            return (MarkupString)Plotly.NET.GenericChart.toChartHTML(combinedChart);
        }

        private int GetTotalOrdersCount()
        {
            return AppDbContext.Orders.Count();
        }

        private float GetMeanOrdersAmount()
        {
            //decimal sumAmount = 0M;
            //foreach (var item in AppDbContext.Orders.ToList())
            //{
            //    sumAmount += item.TotalAmount;
            //}
            decimal sumAmount = AppDbContext.Orders.Sum(o => o.TotalAmount);
            return (float)sumAmount / totalOrdersCount;
        }

        private void ActualizeCustomersGroups()             //This process should be perform after order is finished, but we don't have this feature yet
        {
            //var groupedOrders = Orders
            //    .GroupBy(o => o.Customer)
            //    .Select(c => new
            //    {
            //        Customer = c.Key,
            //        Orders = Orders.Where(o => o.Customer == c.Key && o.Status == OrderStatus.Finished),
            //    });

            Dictionary<Customer, decimal> customersDict = new Dictionary<Customer, decimal>();
            foreach (var customer in AppDbContext.Customers.ToList())
            {
                customersDict.Add(customer, customer.Orders.Where(o => o.Status == OrderStatus.Finished).Sum(o => o.TotalAmount));
            }
            var sortedCustomersDict = customersDict.OrderByDescending(c => c.Value);
            //var finishedOrders = AppDbContext.Orders.Where(o => o.Status == OrderStatus.Finished);
            //var sortedOrders = finishedOrders.OrderByDescending(o => o.TotalAmount).ToList();
            var indexOftreshold = (int)(0.10 * sortedCustomersDict.Count());
            var treshold = sortedCustomersDict.ToArray()[indexOftreshold].Value;
            foreach (var customer in AppDbContext.Customers.ToList())
            {
                if (customer.Orders.Where(o => o.Status == OrderStatus.Finished).Sum(o => o.TotalAmount) > treshold)
                {
                    customer.CustomerGroup = CustomerGroup.VIP;
                    continue;
                }

                bool boughtInLastMonth = customer.Orders.ToList().Exists(o => o.OrderDate.Month == DateTime.Now.AddMonths(-1).Month);
                bool boughtInLastprelastMonth = customer.Orders.ToList().Exists(o => o.OrderDate.Month == DateTime.Now.AddMonths(-2).Month);
                if (boughtInLastMonth && boughtInLastprelastMonth)
                {
                    customer.CustomerGroup = CustomerGroup.Regular;
                }
            }
            AppDbContext.SaveChanges();
        }

        private int GetRegularCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.Regular).Count();
        }

        private int GetVipCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.VIP).Count();
        }

        private void SeparateProducts()
        {
            //List<int> popularProducts = new List<int>();
            //List<int> neutralProducts = new List<int>();
            //List<int> unpopularProducts = new List<int>();

            foreach (var product in groupedProducts)
            {
                if (product.DatesOrdered.Count >= 2)
                {
                    var currMonth = product.DatesOrdered[product.DatesOrdered.Count - 1];
                    var lastMonth = product.DatesOrdered[product.DatesOrdered.Count - 2];
                    var orderDiff = currMonth.OrderCount - lastMonth.OrderCount;
                    if (orderDiff > 0)
                        popularProducts.Add(product.ProductName);
                    else if (orderDiff == 0)
                        neutralProducts.Add(product.ProductName);
                    else
                        unpopularProducts.Add(product.ProductName);
                }
            }
        }

        //private MarkupString GetMetricsTable()
        //{
        //    var headers = new[] { "Column 1", "Column 2", "Column 3" };
        //    var cells = new[,] {
        //        { "Row 1 Col 1", "Row 1 Col 2", "Row 1 Col 3" },
        //        { "Row 2 Col 1", "Row 2 Col 2", "Row 2 Col 3" },
        //        { "Row 3 Col 1", "Row 3 Col 2", "Row 3 Col 3" }
        //    };

        //    var table = Plotly.NET.ChartDomain.Chart.Table<string, string, string>(headers, cells);
        //    //    ChartDomain.Chart.Table<string[], float[]>(
        //    //        headerValues: header,
        //    //        cellsValues: cells);
        //    //    header.SetValue("row 1", "průměrný počet objednávek za den");
        //    //    cells.SetValue("row 2", meanOrdersValue);
        //    //    var genericChart = Chart.Table(
        //    //        header: header,
        //    //        cells: cells);

        //    return (MarkupString)Plotly.NET.GenericChart.toChartHTML(genericChart);
        //}
    }
}
