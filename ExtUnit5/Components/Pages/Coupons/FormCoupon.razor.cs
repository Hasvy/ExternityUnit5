using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Coupons
{
    public partial class FormCoupon : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        //[Parameter] public string? CouponId { get; set; }
        [Parameter] public string FormName { get; set; } = null!;

        private AppDbContext AppDbContext { get; set; } = null!;
        private Coupon? coupon = new Coupon();
        private int? SelectedCustomerId { get; set; }

        private int? SelectedProductId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
        }

        private async Task Submit()
        {
            AppDbContext.Coupons.Add(coupon);
            await AppDbContext.SaveChangesAsync();
            NavigationManager.NavigateTo("/coupons");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
