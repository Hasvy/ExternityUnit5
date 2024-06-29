using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.CategoryPages
{
    public partial class CategoryEdit : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public string? CategoryId { get; set; }

        private Category categoryToEdit = new Category();

        protected override async Task OnParametersSetAsync()
        {
            if (int.TryParse(CategoryId, out int categoryId))
            {
                using (var context = DbContextFactory.CreateDbContext())
                {
                    categoryToEdit = await context.Categories.FindAsync(categoryId);
                }
                await base.OnParametersSetAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                context.Categories.Update(categoryToEdit);
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/categories");
        }
    }
}
