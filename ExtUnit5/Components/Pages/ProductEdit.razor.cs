using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages
{
    public partial class ProductEdit : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public string? ProductId { get; set; }

        private Product productToEdit = new Product();

        protected override async Task OnParametersSetAsync()
        {
            if (int.TryParse(ProductId, out int productId))
            {
                using (var context = DbContextFactory.CreateDbContext())
                {
                    productToEdit = await context.Products.FindAsync(productId);
                    //if (product is not null)
                    //{
                    //    ProductToEdit = product;
                    //}
                }
                await base.OnParametersSetAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                //context.Products.Add(ProductToEdit);
                context.Products.Update(productToEdit);
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/products");
        }
    }
}
