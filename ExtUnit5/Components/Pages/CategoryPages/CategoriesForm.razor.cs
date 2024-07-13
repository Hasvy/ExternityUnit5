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
        [Parameter] public string FormName { get; set; } = null!;

        private Category category = new Category();

        protected override async Task OnInitializedAsync()
        {
            if (CategoryId is not null)
            {
                int.TryParse(CategoryId, out int categoryId);
                using (var context = DbContextFactory.CreateDbContext())
                {
                    category = await context.Categories.FindAsync(categoryId);
                }
                await base.OnParametersSetAsync();
            }
        }

        private async Task Submit()
        {
            using (var context = DbContextFactory.CreateDbContext())
            {
                if (CategoryId is not null)
                {
                    context.Categories.Update(category);
                }
                else
                {
                    context.Categories.Add(category);
                }
                await context.SaveChangesAsync();
            }

            NavigationManager.NavigateTo("/categories");
        }
    }
}
