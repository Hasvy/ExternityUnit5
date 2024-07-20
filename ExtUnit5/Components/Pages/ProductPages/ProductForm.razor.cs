using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class ProductForm : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public string? ProductId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;
        private AppDbContext AppDbContext { get; set; } = null!;
        public int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                _selectedCategoryId = value;
                product.Category = allCategories.Find(c => c.Id == _selectedCategoryId)!;
            }
        }
        private Product product = new Product();
        private List<Category> allCategories = new List<Category>();
        private int? _selectedCategoryId;

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            allCategories = await AppDbContext.Categories.ToListAsync();
            if (ProductId is not null)
            {
                int.TryParse(ProductId, out int productId);
                var foundedProduct = await AppDbContext.Products.FindAsync(productId);
                if (foundedProduct is not null)
                {
                    product = foundedProduct;
                    SelectedCategoryId = product.Category.Id;
                }
                else
                    Console.WriteLine("Product has not found");
            }
            await base.OnInitializedAsync();
        }

        private async Task Submit()
        {
            if (ProductId is not null)
            {
                AppDbContext.Products.Update(product);
            }
            else
            {
                AppDbContext.Products.Add(product);
            }
            await AppDbContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/products");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
