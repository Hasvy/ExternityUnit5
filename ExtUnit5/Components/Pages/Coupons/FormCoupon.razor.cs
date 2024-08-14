using Database;
using ExtUnit5.Entities;
using ExtUnit5.Services;
using Mailhog;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ExtUnit5.Components.Pages.Coupons
{
    public partial class FormCoupon : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] CodeGeneratorService CodeGenerator { get; set; } = null!;
        [Inject] EmailService EmailService { get; set; } = null!;
        [Parameter] public string FormName { get; set; } = null!;

        public int? SelectedCustomerId
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

        public int? SelectedProductId
        {
            get => _selectedProductId;
            set
            {
                _selectedProductId = value;
                coupon.Product = AppDbContext.Products.First(c => c.Id == _selectedProductId);
                coupon.PriceWithDiscount = GetPriceWithDiscount(coupon);
            }
        }

        public int ExpireInDays
        {
            get => _expireInDays;
            set
            {
                _expireInDays = value;
                if (_expireInDays > 0)
                {
                    coupon.ExpireDate = DateTime.Now.AddDays(_expireInDays).Date;
                }
            }
        }

        private AppDbContext AppDbContext { get; set; } = null!;
        private Coupon coupon = new Coupon();
        private List<string> couponCodes = new List<string>();
        private string errorMessage = string.Empty;
        private int? _selectedCustomerId;
        private int? _selectedProductId;
        private int _expireInDays = 1;

        protected override async Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            couponCodes = AppDbContext.Coupons.Select(c => c.Code).ToList();
            await base.OnInitializedAsync();
        }

        private List<Customer> GetCustomersWithDiscount()
        {
            return AppDbContext.Customers.Where(c => c.CustomerGroup != CustomerGroup.Basic).OrderBy(c => c.FirstName).ToList();
        }

        private void GenerateCouponeCode()
        {
            int attempts = 0;
            int maxAttempts = 5;
            errorMessage = string.Empty;

            do
            {
                coupon.Code = CodeGenerator.GenerateCode(12);
            } while (couponCodes.Contains(coupon.Code) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                errorMessage = "Failed to generate a unique code after several attempts.";
            }
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
            AppDbContext.SaveChanges();
            //await EmailService.SendEmailAsync(coupon.Customer.Email, "Discount coupon", "You have new discount coupon on ecommerce shop!");
            NavigationManager.NavigateTo("/coupons");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
        }
    }
}
