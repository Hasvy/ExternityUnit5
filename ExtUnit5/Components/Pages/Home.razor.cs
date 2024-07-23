using Database;
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

namespace ExtUnit5.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        //[Inject] NavigationManager NavigationManager { get; set; } = null!;
        private AppDbContext AppDbContext { get; set; } = null!;
        private MarkupString OrdersChart;
        private MarkupString MetricsTable;
        private float meanOrdersValue;
        private List<GroupedProduct> groupedProducts;
        private List<string> popularProducts = new List<string>();
        private List<string> neutralProducts = new List<string>();
        private List<string> unpopularProducts = new List<string>();
        private bool _isLoading;
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            AppDbContext = await DbContextFactory.CreateDbContextAsync();
            OrdersChart = GetOrdersChart();
            //MetricsTable = GetMetricsTable();
            await base.OnInitializedAsync();
            _isLoading = false;
        }

        private MarkupString GetOrdersChart()
        {
            var orders = AppDbContext.Orders.ToList();

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

            var groupedOrders = orders
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

            var genericChart = Plotly.NET.CSharp.Chart.Line<DateTime, int, string>(
                y: groupedOrders.Select(g => g.Orders.Count),
                x: groupedOrders.Select(g => g.OrderDate)
                )
                .WithSize(800, 400)
                .WithTraceInfo("Order Counts", ShowLegend: true)
                .WithXAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Date"))
                .WithYAxisStyle<int, DateTime, string>(Title: Plotly.NET.Title.init("Order Count")
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
