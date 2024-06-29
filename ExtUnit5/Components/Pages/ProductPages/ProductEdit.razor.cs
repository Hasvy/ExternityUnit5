using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class ProductEdit : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public string? ProductId { get; set; }

        private Product productToAdd = new Product();
        private List<Category> allCategories = new List<Category>();
        private int selectedCategoryId;

        protected override async Task OnInitializedAsync()
        {
            if (int.TryParse(ProductId, out int productId))
            {
                using (var context = DbContextFactory.CreateDbContext())
                {
                    productToAdd = await context.Products.FindAsync(productId);
                    allCategories = await context.Categories.ToListAsync();
                }
                selectedCategoryId = productToAdd.Category.Id;
                await base.OnInitializedAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                if (productToAdd.Category.Id != selectedCategoryId)
                {
                    var newCategory = await context.Categories.FindAsync(selectedCategoryId);
                    productToAdd.Category = newCategory;
                }
                context.Products.Update(productToAdd);
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/products");
        }
    }
}
