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

        private readonly int _itemsPerPage = 10;
        private int _currentPage = 1;
        private List<Order> OrdersOnPage => AllOrders.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();

        private string? CustomerFilter 
        {
            get => _customerFilterInput;
            set
            {
                _customerFilterInput = value;
                ApplyFilters();
            }
        }

        private OrderStatus? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                ApplyFilters();
            }
        }

        private OrderStatus? _selectedStatus;
        private string? _customerFilterInput;

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

        private void ApplyFilters()
        {
            AllOrders = AppDbContext.Orders.Where(o =>
                (SelectedStatus == null || o.Status == SelectedStatus) &&
                (CustomerFilter == null || (o.Customer.FirstName + " " + o.Customer.LastName).Contains(CustomerFilter))
            ).ToList();
        }

        private void HandlePageChanged(int newPageNumber)
        {
            _currentPage = newPageNumber;
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
