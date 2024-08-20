using Database;
using ExtUnit5.Entities;
using ExtUnit5.Entities.Grouping;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Plotly.Blazor.Traces;
using Plotly.Blazor;
using Plotly.Blazor.Traces.ScatterLib;
using Line = Plotly.Blazor.Traces.ScatterLib.Line;
using Plotly.Blazor.Traces.ScatterLib.LineLib;

namespace ExtUnit5.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Customer> Customers { get; set; } = new List<Customer>();

        private AppDbContext AppDbContext { get; set; } = null!;
        private float avgOrdersValue;
        private float meanOrdersAmount;
        private int totalOrdersCount;
        private int newCustomersCount;
        private int regularCustomersCount;
        private int vipCustomersCount;
        private bool _isLoading;

        private Plotly.Blazor.Config? ordersChartConfig;
        private Plotly.Blazor.Layout? ordersChartLayout;
        private IList<ITrace>? ordersChartData;

        private Plotly.Blazor.Config? newCustomersConfig;
        private Plotly.Blazor.Layout? newCustomersLayout;
        private IList<ITrace>? newCustomersData;

        private Plotly.Blazor.Config? productTrendsConfig;
        private Plotly.Blazor.Layout? productTrendsLayout;
        private IList<ITrace>? productTrendsData;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            AppDbContext = await DbContextFactory.CreateDbContextAsync();

            Orders = AppDbContext.Orders.ToList();
            Customers = AppDbContext.Customers.ToList();

            CreateOrdersChart();
            CreateProductTrendsChart();
            CreateNewCustomersChart();

            totalOrdersCount = GetTotalOrdersCount();
            meanOrdersAmount = GetMeanOrdersAmount();
            newCustomersCount = GetNewCustomersCount();
            regularCustomersCount = GetRegularCustomersCount();
            vipCustomersCount = GetVipCustomersCount();

            await base.OnInitializedAsync();
            _isLoading = false;
        }

        #region Charts

        private void CreateOrdersChart()
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

            ordersChartConfig = new Plotly.Blazor.Config();
            ordersChartLayout = new Plotly.Blazor.Layout();
            ordersChartData = new List<ITrace>
            {
                new Scatter
                {
                    Name = "Orders Count",
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    X = monthlyGroupedOrders.Select(g => (object)g.Month).ToList(),
                    Y = monthlyGroupedOrders.Select(g => (object)g.OrderCount).ToList(),
                    Line = new Line { Shape = ShapeEnum.Spline },
                    Fill = FillEnum.ToZeroY
                }
            };

            #region Plotly.Net Orders Chart
            //var genericChart = Chart2D.Chart.SplineArea<DateTime, int, string>(
            //    y: monthlyGroupedOrders.Select(g => g.OrderCount),
            //    x: monthlyGroupedOrders.Select(g => g.Month)
            //    )
            //    .WithSize(800, 400)
            //    .WithTraceInfo("Orders Count", ShowLegend: false)
            //    .WithXAxisStyle(title: Title.init("Month"))
            //    .WithYAxisStyle(title: Title.init("Orders Count"));

            //return (MarkupString)GenericChart.toChartHTML(genericChart);
            #endregion
        }

        private void CreateProductTrendsChart()
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
                .OrderByDescending(g => g.DatesOrdered.Count)
                .ToList();

            productTrendsConfig = new Plotly.Blazor.Config();
            productTrendsLayout = new Plotly.Blazor.Layout();
            productTrendsData = new List<ITrace>();

            foreach (var productData in groupedProducts)
            {
                productTrendsData.Add(new Scatter
                {
                    Name = productData.ProductName ?? "Product " + productData.ProductId,
                    X = productData.DatesOrdered.Select(d => (object)d.Month).ToList(),
                    Y = productData.DatesOrdered.Select(d => (object)d.OrderCount).ToList(),
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    Line = new Line { Shape = ShapeEnum.Spline }
                });
            }

            #region Plotly.Net Product Trends Chart

            //List<GenericChart> charts = new List<GenericChart>();

            //foreach (var productData in groupedProducts)
            //{
            //    charts.Add(Chart2D.Chart.Spline<DateTime, int, string>(
            //        x: productData.DatesOrdered.Select(d => d.Month),
            //        y: productData.DatesOrdered.Select(d => d.OrderCount),
            //        Name: productData.ProductName ?? "Product " + productData.ProductId.ToString()
            //        ));
            //}

            //var combinedChart = Chart.Combine(charts)
            //    .WithSize(800, 400)
            //    .WithXAxisStyle(title: Title.init("Date"))
            //    .WithYAxisStyle(title: Title.init("Order Count"));

            //return (MarkupString)GenericChart.toChartHTML(combinedChart);
            #endregion
        }

        private void CreateNewCustomersChart()
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

            var newCustomersForYear = monthlyGroupedCustomers.Where(g => g.Month > DateTime.Today.AddMonths(-12));

            newCustomersConfig = new Plotly.Blazor.Config();
            newCustomersLayout = new Plotly.Blazor.Layout();
            newCustomersData = new List<ITrace>
            {
                new Scatter
                {
                    Name = "Customers Count",
                    Mode = ModeFlag.Lines | ModeFlag.Markers,
                    X = newCustomersForYear.Select(g => (object)g.Month).ToList(),
                    Y = newCustomersForYear.Select(g => (object)g.CustomersCount).ToList(),
                    Line = new Line { Shape = ShapeEnum.Spline },
                    Fill = FillEnum.ToZeroY
                }
            };

            #region Plotly.Net New Customers Chart
            //var newCustomersChart = Chart2D.Chart.Column<int, DateTime, string, string, string> (
            //        Keys: newCustomersForYear.Select(g => g.Month).ToList(),
            //        values: monthlyGroupedCustomers.Select(g => g.CustomersCount)
            //    )
            //    .WithSize(1200, 400)
            //    .WithTraceInfo("Customers Count", ShowLegend: false)
            //    .WithXAxisStyle(title: Title.init("Month"))
            //    .WithYAxisStyle(title: Title.init("Customers Count"));

            //return (MarkupString)GenericChart.toChartHTML(newCustomersChart);
            #endregion
        }

        #endregion

        #region Metrics

        private int GetNewCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.RegistrationDate > DateTime.Today.AddMonths(-1)).Count();
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

        private int GetRegularCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.Regular).Count();
        }

        private int GetVipCustomersCount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup == CustomerGroup.VIP).Count();
        }

        private float GetPopularityDiff(float popularity)
        {
            return (float)Math.Round((popularity - 1) * 100, 2);
        }

        #endregion
    }
}