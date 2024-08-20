using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExtUnit5.Entities
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kód je povinný")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Zákazník je povinný")]
        public virtual Customer Customer { get; set; } = null!;
        [Required(ErrorMessage = "Produkt je povinný")]
        public virtual Product Product { get; set; } = null!;
        public float Discount { get; set; }
        public float PriceWithDiscount { get; set; }
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddDays(1).Date;

        public Coupon()
        {

        }

        public Coupon(string code, Customer customer, Product product, DateTime expireDate)
        {
            Code = code;
            Customer = customer;
            Product = product;
            Discount = GetCouponDiscount(customer);
            PriceWithDiscount = GetPriceWithDiscount(product, Discount);
            ExpireDate = expireDate;
        }

        private float GetCouponDiscount(Customer customer)
        {
            if (customer is not null)
            {
                return customer.CustomerGroup switch
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

        private float GetPriceWithDiscount(Product product, float discount)
        {
            float priceWithDiscount = (float)Math.Round(product.Price - discount, 2);
            if (priceWithDiscount < 1)
                Discount = product.Price - 1;
            return (float)Math.Round(product.Price - discount, 2);
        }
    }
}
