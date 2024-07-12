using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.ProductPages
{
    public partial class ProductEdit : ComponentBase
    {
        [Parameter] public string? ProductId { get; set; }
    }
}
