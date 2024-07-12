using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.CategoryPages
{
    public partial class CategoryEdit : ComponentBase
    {
        [Parameter] public string? CategoryId { get; set; }
    }
}
