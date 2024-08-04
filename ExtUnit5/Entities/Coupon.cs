using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ExtUnit5.Entities
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kód je povinný")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Zákazník je povinný")]
        public virtual Customer Customer { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public float Discount { get; set; }
        public float PriceWithDiscount { get; set; }
    }
}
