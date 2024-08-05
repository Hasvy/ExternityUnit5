using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ExtUnit5.Components.Pages.Coupons
{
    public partial class FormCoupon : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Parameter] public string FormName { get; set; } = null!;

        private AppDbContext AppDbContext { get; set; } = null!;

        private Coupon coupon = new Coupon();

        private int? SelectedCustomerId
        {
            get => _selectedCustomerId;
            set
            {
                _selectedCustomerId = value;
                coupon.Customer = AppDbContext.Customers.First(c => c.Id == _selectedCustomerId);
                coupon.Discount = GetCouponDiscount();
                if (coupon.Product is not null)
                    coupon.PriceWithDiscount = GetPriceWithDiscount(coupon);
            }
        }

        private int? SelectedProductId
        {
            get => _selectedProductId;
            set
            {
                _selectedProductId = value;
                coupon.Product = AppDbContext.Products.First(c => c.Id == _selectedProductId);
                coupon.PriceWithDiscount = GetPriceWithDiscount(coupon);
            }
        }

        private int? _selectedCustomerId;
        private int? _selectedProductId;

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            await base.OnInitializedAsync();
        }

        private List<Customer> GetCustomersWithDiscount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup != CustomerGroup.Basic).OrderBy(c => c.FirstName).ToList();
        }

        private void GenerateCouponeCode()
        {

        }

        private float GetCouponDiscount()
        {
            switch (coupon.Customer.CustomerGroup)
            {
                case CustomerGroup.New:
                    return 0.10f;
                case CustomerGroup.Regular:
                    return 0.20f;
                case CustomerGroup.VIP:
                    return 0.30f;
                default:
                    return 0.00f;
            }
        }

        private float GetPriceWithDiscount(Coupon coupon)
        {
            return (float)Math.Round(coupon.Product.Price - (coupon.Product.Price * coupon.Discount), 2);
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
