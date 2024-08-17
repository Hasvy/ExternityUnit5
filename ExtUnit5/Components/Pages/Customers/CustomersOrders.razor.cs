using Castle.Core.Resource;
using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Customers
{
    public partial class CustomersOrders : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        //[Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public int? CustomerId { get; set; }
        private AppDbContext AppDbContext { get; set; } = null!;
        private Customer? customer = new Customer();

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            if (CustomerId is not null)
            {
                //int.TryParse(CustomerId, out int customerId);
                customer = AppDbContext.Customers.Find(CustomerId);
            }
            return base.OnInitializedAsync();
        }
    }
}
