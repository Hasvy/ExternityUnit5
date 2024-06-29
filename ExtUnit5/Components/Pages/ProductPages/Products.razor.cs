using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class Products : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        public List<Product> AllProducts { get; set; } = new List<Product>();

        protected override Task OnInitializedAsync()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                AllProducts = context.Products.ToList();
            }
            return base.OnInitializedAsync();
        }

        private void Edit(int id)
        {
            NavigationManager.NavigateTo($"/edit-productid-{id}");
        }

        private async Task Delete(int id)
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                var productToDelete = context.Products.SingleOrDefault(p => p.Id == id);
                if (productToDelete != null)
                {
                    context.Products.Remove(productToDelete);
                }
                await context.SaveChangesAsync();
            }
            AllProducts = AllProducts.Where(p => p.Id != id).ToList();
        }

        private void RedirectToAddProduct()
        {
            NavigationManager.NavigateTo($"/addproduct");
        }
    }
}
