using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Orders
{
    public partial class Orders : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<Order> AllOrders { get; set; } = new List<Order>();
        private AppDbContext AppDbContext { get; set; } = null!;

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            AllOrders = AppDbContext.Orders.ToList();
            return base.OnInitializedAsync();
        }

        private void RedirectToEdit(int id)
        {
            NavigationManager.NavigateTo($"/detail-orderid-{id}");
        }

        private async Task DeleteOrder(Order order)
        {
            AppDbContext.Orders.Remove(order);
            await AppDbContext.SaveChangesAsync();
            AllOrders.Remove(order);
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
