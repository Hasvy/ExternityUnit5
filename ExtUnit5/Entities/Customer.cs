using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace ExtUnit5.Entities
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Jmeno je povinné")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; }

        [AllowNull]
        [Phone]
        public string? PhoneNumber { get; set; }

        [AllowNull]
        public string? Address { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
