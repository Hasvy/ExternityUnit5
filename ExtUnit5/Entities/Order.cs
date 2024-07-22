using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExtUnit5.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual Customer Customer { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount
        {
            get
            {
                _totalAmount = OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;
            }
        }

        private decimal _totalAmount;


        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.New;
    }

    public enum OrderStatus
    {
        New, 
        Processing,
        Finished,
        Cancelled
    }
}
