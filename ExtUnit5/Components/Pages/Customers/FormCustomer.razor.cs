using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Customers
{
    public partial class FormCustomer : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public string? CustomerId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;

        private Customer customer = new Customer();

        protected override async Task OnInitializedAsync()
        {
            if (CustomerId is not null)
            {
                int.TryParse(CustomerId, out int customerId);
                using (var context = DbContextFactory.CreateDbContext())
                {
                    customer = await context.Customers.FindAsync(customerId);
                }
                await base.OnParametersSetAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                if (CustomerId is not null)
                {
                    context.Customers.Update(customer);
                }
                else
                {
                    context.Customers.Add(customer);
                }
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/customers");
        }
    }
}
