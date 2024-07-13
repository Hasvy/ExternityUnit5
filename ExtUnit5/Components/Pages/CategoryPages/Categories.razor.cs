using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtUnit5.Components.Pages.CategoryPages
{
    public partial class Categories : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<Category> AllCategories { get; set; } = new List<Category>();
        private AppDbContext AppDbContext { get; set; } = null!;

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            AllCategories = AppDbContext.Categories.ToList();
            return base.OnInitializedAsync();
        }

        private void Edit(int id)
        {
            NavigationManager.NavigateTo($"/edit-categoryid-{id}");
        }

        private async Task Delete(Category category)
        {
            AppDbContext.Categories.Remove(category);
            await AppDbContext.SaveChangesAsync();
            AllCategories.Remove(category);
        }

        private void RedirectToAddCategory()
        {
            NavigationManager.NavigateTo($"/addcategory");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
