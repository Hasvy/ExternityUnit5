using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Orders
{
    public partial class OrderDetail : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public string? OrderId { get; set; }
        private AppDbContext AppDbContext { get; set; } = null!;
        private List<OrderItem> orderItems = new List<OrderItem>();
        private Customer customer = new Customer();

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            if (OrderId is not null)
            {
                int.TryParse(OrderId, out int orderId);
                orderItems = AppDbContext.OrderItems.Where(o => o.Order.Id == orderId).ToList();
                var order = AppDbContext.Orders.Find(orderId);
                if (order is not null)
                    customer = order.Customer;
            }
            return base.OnInitializedAsync();
        }
    }
}
