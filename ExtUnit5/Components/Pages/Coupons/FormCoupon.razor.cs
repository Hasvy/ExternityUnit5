using Database;
using ExtUnit5.Entities;
using ExtUnit5.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.Coupons
{
    public partial class FormCoupon : ComponentBase, IDisposable
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] CodeGeneratorService CodeGenerator { get; set; } = null!;
        [Parameter] public string FormName { get; set; } = null!;

        public int? SelectedCustomerId
        {
            get => _selectedCustomerId;
            set
            {
                _selectedCustomerId = value;
                coupon.Customer = AppDbContext.Customers.First(c => c.Id == _selectedCustomerId);
                if(!IsFixedAmount)
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

        public bool IsFixedAmount
        {
            get => _isFixedAmount;
            set
            {
                _isFixedAmount = value;
                if (_isFixedAmount == false)
                    coupon.Discount = GetCouponDiscount();
                if (coupon.Product is not null)
                {
                    coupon.Discount = Discount;
                    coupon.PriceWithDiscount = GetPriceWithDiscount(coupon);
                }
            }
        }

        public float Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                coupon.Discount = _discount;
                if (coupon.Product is not null)
                    coupon.PriceWithDiscount = GetPriceWithDiscount(coupon);
            }
        }

        private AppDbContext AppDbContext { get; set; } = null!;
        private readonly Coupon coupon = new Coupon();
        private List<string> couponCodes = new List<string>();
        private string errorMessage = string.Empty;
        private int? _selectedCustomerId;
        private int? _selectedProductId;
        private int _expireInDays = 1;
        private bool _isFixedAmount = false;
        private float _discount = 0;

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
            if (coupon.Customer is not null)
            {
                return coupon.Customer.CustomerGroup switch
                {
                    CustomerGroup.New => 0.10f,
                    CustomerGroup.Regular => 0.20f,
                    CustomerGroup.VIP => 0.30f,
                    _ => 0.00f,
                };
            }
            else
                return 0.00f;
        }

        private float GetPriceWithDiscount(Coupon coupon)
        {
            if (!IsFixedAmount)
                return (float)Math.Round(coupon.Product.Price - (coupon.Product.Price * coupon.Discount), 2);
            else
            {
                float priceWithDiscount = (float)Math.Round(coupon.Product.Price - coupon.Discount, 2);
                if (priceWithDiscount < 1)
                    Discount = coupon.Product.Price - 1;
                return (float)Math.Round(coupon.Product.Price - coupon.Discount, 2);
            }
        }

        private void Submit()
        {
            AppDbContext.Coupons.Add(coupon);
            AppDbContext.SaveChanges();
            //await EmailService.SendEmailAsync(coupon.Customer.Email, "Discount coupon", "You have new discount coupon on ecommerce shop!");
            NavigationManager.NavigateTo("/coupons");
        }

        public void Dispose()
        {
            AppDbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
