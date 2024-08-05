using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Coupons
{
    public partial class Coupons : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private List<Coupon> AllCoupons { get; set; } = new List<Coupon>();
        private AppDbContext AppDbContext { get; set; } = null!;
        private List<Coupon> CouponsOnPage => AllCoupons.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();

        private int _currentPage = 1;
        private int _itemsPerPage = 10;

        private void RedirectToAddCoupon()
        {
            NavigationManager.NavigateTo($"/addcoupon");
        }

        private async Task DeleteCoupon(Coupon coupon)
        {
            AppDbContext.Coupons.Remove(coupon);
            await AppDbContext.SaveChangesAsync();
            AllCoupons.Remove(coupon);
        }

        private void HandlePageChanged(int newPageNumber)
        {
            _currentPage = newPageNumber;
        }
    }
}
