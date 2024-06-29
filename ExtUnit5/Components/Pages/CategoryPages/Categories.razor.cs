using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ExtUnit5.Components.Pages.CategoryPages
{
    public partial class Categories : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        public List<Category> AllCategories { get; set; } = new List<Category>();

        protected override Task OnInitializedAsync()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                AllCategories = context.Categories.ToList();
            }
            return base.OnInitializedAsync();
        }

        private void Edit(int id)
        {
            NavigationManager.NavigateTo($"/edit-categoryid-{id}");
        }

        private async Task Delete(int id)
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                var categoryToDelete = context.Categories.SingleOrDefault(c => c.Id == id);
                if (categoryToDelete != null)
                {
                    context.Categories.Remove(categoryToDelete);
                }
                await context.SaveChangesAsync();
            }
            AllCategories = AllCategories.Where(c => c.Id != id).ToList();
        }

        private void RedirectToAddCategory()
        {
            NavigationManager.NavigateTo($"/addcategory");
        }
    }
}
