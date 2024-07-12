using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.CategoryPages
{
    public partial class CategoriesForm : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;

        [Inject] NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public string? CategoryId { get; set; }

        private Category categoryToAdd = new Category();

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                context.Categories.Add(categoryToAdd);
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/categories");
        }
    }
}
