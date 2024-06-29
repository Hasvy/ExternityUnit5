using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class ProductAdd : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public string? ProductId { get; set; }

        private Product productToAdd = new Product();
        private List<Category> allCategories = new List<Category>();
        private int selectedCategoryId;

        protected override async Task OnInitializedAsync()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                allCategories = await context.Categories.ToListAsync();
            }
            productToAdd.Category = allCategories.First();
            selectedCategoryId = productToAdd.Category.Id;
            await base.OnInitializedAsync();
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                var newCategory = await context.Categories.FindAsync(selectedCategoryId);
                productToAdd.Category = newCategory;
                context.Products.Add(productToAdd);
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/products");
        }
    }
}
