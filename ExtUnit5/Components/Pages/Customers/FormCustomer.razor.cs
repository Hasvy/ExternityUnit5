using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Customers
{
    public partial class FormCustomer : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public int? CustomerId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;

        private AppDbContext AppDbContext { get; set; } = null!;
        private Customer customer = new Customer();

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();

            if (CustomerId is not null)
            {
                //int.TryParse(CustomerId, out int customerId);
                var customer = await AppDbContext.Customers.FindAsync(CustomerId);
                if (customer is not null)
                    this.customer = customer;
                else
                    Console.WriteLine($"Customer with ID {CustomerId} was not found in database.");
            }
            await base.OnInitializedAsync();
        }

        private void Submit()
        {
            if (customer is not null)
            {
                if (CustomerId is not null)
                {
                    AppDbContext.Customers.Update(customer);
                }
                else
                {
                    AppDbContext.Customers.Add(customer);
                }
            }
            AppDbContext.SaveChanges();
            NavigationManager.NavigateTo("/customers");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
