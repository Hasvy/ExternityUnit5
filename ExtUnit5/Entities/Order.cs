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

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

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

    public static class EnumHelper
    {
        private static readonly Random _random = new Random();

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }
    }
}
