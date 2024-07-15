using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Customers
{
    public partial class Customers : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<Customer> AllCustomers { get; set; } = new List<Customer>();
        private AppDbContext AppDbContext { get; set; } = null!;

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            AllCustomers = AppDbContext.Customers.ToList();
            return base.OnInitializedAsync();
        }
        private void RedirectToAddCustomer()
        {
            NavigationManager.NavigateTo($"/addcustomer");
        }

        private void RedirectToEdit(int id)
        {
            NavigationManager.NavigateTo($"/edit-customerid-{id}");
        }

        private async Task DeleteCustomer(Customer customer)
        {
            AppDbContext.Customers.Remove(customer);
            await AppDbContext.SaveChangesAsync();
            AllCustomers.Remove(customer);
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
