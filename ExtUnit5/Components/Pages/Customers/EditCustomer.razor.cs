using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Customers
{
    public partial class EditCustomer : ComponentBase
    {
        [Parameter] public string? CustomerId { get; set; }
    }
}
