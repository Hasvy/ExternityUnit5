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

        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private List<Customer> CustomersOnPage => AllCustomers.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();

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

        private void HandlePageChanged(int newPageNumber)
        {
            _currentPage = newPageNumber;
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
