using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtUnit5.Entities
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual Order Order { get; set; } = null!;

        [Required]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
        //public decimal UnitPrice
        //{
        //    get
        //    {
        //        _unitPrice = Quantity * (decimal)Product.Price;
        //        return _unitPrice;
        //    }
        //    set
        //    {
        //        _unitPrice = value;
        //    }
        //}

        //private decimal _unitPrice;

        //public void CalculateUnitPrice()
        //{
        //    UnitPrice = Quantity * (decimal)Product.Price;
        //}
    }
}
