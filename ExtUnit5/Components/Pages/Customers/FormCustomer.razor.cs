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
        [Parameter] public string? CustomerId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;

        private AppDbContext AppDbContext { get; set; } = null!;
        private Customer? customer = new Customer();

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();

            if (CustomerId is not null)
            {
                int.TryParse(CustomerId, out int customerId);
                customer = await AppDbContext.Customers.FindAsync(customerId);
                await base.OnParametersSetAsync();
            }
        }

        private async Task Submit()
        {
            if (CustomerId is not null)
            {
                AppDbContext.Customers.Update(customer);
            }
            else
            {
                AppDbContext.Customers.Add(customer);
            }
            await AppDbContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/customers");
            //if (IsEmailUnique(customer!.Email))
            //{

            //}
            //else
            //{

            //}
        }

        //private bool IsEmailUnique(string email)
        //{
        //    return AppDbContext.Customers.FirstOrDefault(c => c.Email == email) == (null) ? true : false;
        //}

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
