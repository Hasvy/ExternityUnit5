using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Orders
{
    public partial class OrderDetail : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Parameter] public int? OrderId { get; set; }
        private AppDbContext AppDbContext { get; set; } = null!;
        private List<OrderItem> orderItems = new List<OrderItem>();
        private Customer customer = new Customer();

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            if (OrderId is not null)
            {
                //int.TryParse(OrderId, out int orderId);
                var order = await AppDbContext.Orders.FindAsync(OrderId);

                if (order is not null)
                {
                    orderItems = order.OrderItems.ToList();
                    customer = order.Customer;
                }
            }
            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
