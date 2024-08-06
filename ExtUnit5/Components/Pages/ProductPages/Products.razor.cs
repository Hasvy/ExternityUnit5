using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class Products : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<Product> AllProducts { get; set; } = new List<Product>();
        private AppDbContext AppDbContext { get; set; } = null!;
        private List<Product> ProductsOnPage => AllProducts.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();

        private int _currentPage = 1;
        private int _itemsPerPage = 10;

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            AllProducts = AppDbContext.Products.ToList();
            return base.OnInitializedAsync();
        }
        private void RedirectToAddProduct()
        {
            NavigationManager.NavigateTo($"/addproduct");
        }

        private void RedirectToEdit(int id)
        {
            NavigationManager.NavigateTo($"/edit-productid-{id}");
        }

        private async Task DeleteProduct(Product product)
        {
            AppDbContext.Products.Remove(product);
            await AppDbContext.SaveChangesAsync();
            AllProducts.Remove(product);
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
