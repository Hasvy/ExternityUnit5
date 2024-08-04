using Database;
using ExtUnit5.Entities;
using ExtUnit5.Entities.Grouping;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Plotly.NET;

namespace ExtUnit5.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        private List<Order> Orders { get; set; } = new List<Order>();
        private List<Customer> Customers { get; set; } = new List<Customer>();

        private AppDbContext AppDbContext { get; set; } = null!;
        private float avgOrdersValue;
        private float meanOrdersAmount;
        private int totalOrdersCount;
        private int newCustomersCount;
        private int regularCustomersCount;
        private int vipCustomersCount;
        private List<string> popularProducts = new List<string>();
        private List<string> neutralProducts = new List<string>();
        private List<string> unpopularProducts = new List<string>();
        private bool _isLoading;
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            AppDbContext = await DbContextFactory.CreateDbContextAsync();

            Orders = AppDbContext.Orders.ToList();
            Customers = AppDbContext.Customers.ToList();
            ActualizeCustomersGroups();

            totalOrdersCount = GetTotalOrdersCount();
            meanOrdersAmount = GetMeanOrdersAmount();
            newCustomersCount = GetNewCustomersCount();
            regularCustomersCount = GetRegularCustomersCount();
            vipCustomersCount = GetVipCustomersCount();

            await base.OnInitializedAsync();
            _isLoading = false;
        }

        #region Charts

        private MarkupString GetOrdersChart()
        {
            var monthlyGroupedOrders = Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(dg => new MonthlyOrder
                    {
                        Month = new DateTime(dg.Key.Year, dg.Key.Month, 1),
                        OrderCount = dg.Count()
                    })
                .OrderBy(o => o.Month)
                .ToList();

            var avgOrders = (float)monthlyGroupedOrders.Average(o => o.OrderCount);
            avgOrdersValue = (float)Math.Round(avgOrders);

            var genericChart = Chart2D.Chart.SplineArea<DateTime, int, string>(
                y: monthlyGroupedOrders.Select(g => g.OrderCount),
                x: monthlyGroupedOrders.Select(g => g.Month)
                )
                .WithSize(800, 400)
                .WithTraceInfo("Orders Count", ShowLegend: false)
                .WithXAxisStyle(title: Title.init("Month"))
                .WithYAxisStyle(title: Title.init("Orders Count"));

            return (MarkupString)GenericChart.toChartHTML(genericChart);
        }

        private MarkupString GetProductTrendsChart()
        {
            var orderItems = AppDbContext.OrderItems.ToList();
            var groupedProducts = orderItems
                .GroupBy(oi => oi.Product.Id)
                .Select(g => new GroupedProduct
                {
                    ProductId = g.Key,
                    ProductName = AppDbContext.Products.Find(g.Key)?.Name!,
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

            GroupProductsByPopularity(groupedProducts);

            List<GenericChart> charts = new List<GenericChart>();

            foreach (var productData in groupedProducts)
            {
                charts.Add(Chart2D.Chart.Spline<DateTime, int, string>(
                    x: productData.DatesOrdered.Select(d => d.Month),
                    y: productData.DatesOrdered.Select(d => d.OrderCount),
                    Name: productData.ProductName ?? "Product " + productData.ProductId.ToString()
                    ));
            }

            var combinedChart = Chart.Combine(charts)
                .WithSize(1000, 500)
                .WithXAxisStyle(title: Title.init("Date"))
                .WithYAxisStyle(title: Title.init("Order Count"));

            return (MarkupString)GenericChart.toChartHTML(combinedChart);
        }

        private MarkupString GetNewCustomersChart()
        {
            var monthlyGroupedCustomers = Customers
                .GroupBy(c => new { c.RegistrationDate.Year, c.RegistrationDate.Month })
                    .Select(dg => new 
                    {
                        Month = new DateTime(dg.Key.Year, dg.Key.Month, 1),
                        CustomersCount = dg.Count()
                    })
                .OrderBy(c => c.Month)
                .ToList();

            var newCustomersForYear = monthlyGroupedCustomers.Where(g => g.Month.Year == DateTime.Today.AddYears(-1).Year);

            var newCustomersChart = Chart2D.Chart.Column<int, DateTime, string, string, string> (
                    Keys: newCustomersForYear.Select(g => g.Month).ToList(),
                    values: monthlyGroupedCustomers.Select(g => g.CustomersCount)
                )
                .WithSize(1200, 400)
                .WithTraceInfo("Customers Count", ShowLegend: false)
                .WithXAxisStyle(title: Title.init("Month"))
                .WithYAxisStyle(title: Title.init("Customers Count"));

            return (MarkupString)GenericChart.toChartHTML(newCustomersChart);
        }

        #endregion

        #region Metrics

        private int GetNewCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.RegistrationDate.Month == DateTime.Today.Month).Count();
        }

        private int GetTotalOrdersCount()
        {
            return AppDbContext.Orders.Count();
        }

        private float GetMeanOrdersAmount()
        {
            decimal sumAmount = AppDbContext.Orders.Sum(o => o.TotalAmount);
            var roundedSumAmount = Math.Round((float)sumAmount / totalOrdersCount, 2);
            return (float)roundedSumAmount;
        }

        //private float GetMeanOrdersValuePerMonth()
        //{
        //    var totalOrdersNumber = AppDbContext.Orders.Count();
        //    var numberOfMonths = 12;//AppDbContext.Orders.Sum(g => g.OrderDate.Month);
        //    var meanOrdersValueDecimal = Math.Round((float)totalOrdersNumber / numberOfMonths, 1);
        //    return (float)meanOrdersValueDecimal;
        //}

        private int GetRegularCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.Regular).Count();
        }

        private int GetVipCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.VIP).Count();
        }

        #endregion

        private void GroupProductsByPopularity(List<GroupedProduct> groupedProducts)
        {
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

        private void ActualizeCustomersGroups()             //This process should be performed after order is finished, but we don't have this feature yet
        {
            Dictionary<Customer, decimal> customersDict = new Dictionary<Customer, decimal>();
            foreach (var customer in AppDbContext.Customers.ToList())
            {
                customersDict.Add(customer, customer.Orders.Where(o => o.Status == OrderStatus.Finished).Sum(o => o.TotalAmount));
            }
            var sortedCustomersDict = customersDict.OrderByDescending(c => c.Value);
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
                bool boughtInPreLastMonth = customer.Orders.ToList().Exists(o => o.OrderDate.Month == DateTime.Now.AddMonths(-2).Month);
                if (boughtInLastMonth && boughtInPreLastMonth)
                {
                    customer.CustomerGroup = CustomerGroup.Regular;
                }
            }
            AppDbContext.SaveChanges();
        }
    }
}