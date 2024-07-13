using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class ProductForm : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public string? ProductId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;

        private Product product = new Product()!;
        private List<Category> allCategories = new List<Category>();
        private int selectedCategoryId;

        protected override async Task OnInitializedAsync()
        {
            if (ProductId is not null)
            {
                int.TryParse(ProductId, out int productId);
                using (var context = DbContextFactory.CreateDbContext())
                {
                    product = await context.Products.FindAsync(productId);
                    allCategories = await context.Categories.ToListAsync();
                }
                selectedCategoryId = product.Category.Id;
                await base.OnInitializedAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                if (product.Category.Id != selectedCategoryId)
                {
                    var newCategory = await context.Categories.FindAsync(selectedCategoryId);
                    product.Category = newCategory;
                }

                if (ProductId is not null)
                {
                    context.Products.Update(product);
                }
                else
                {
                    context.Products.Add(product);
                }
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/products");
        }
    }
}
