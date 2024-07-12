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

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            AllProducts = AppDbContext.Products.ToList();
            return base.OnInitializedAsync();
        }

        private void RedirectToEdit(int id)
        {
            NavigationManager.NavigateTo($"/edit-productid-{id}");
        }

        private async Task DeleteProduct(Product product)
        {
            AppDbContext.Remove(product);
            await AppDbContext.SaveChangesAsync();
            AllProducts.Remove(product);
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
