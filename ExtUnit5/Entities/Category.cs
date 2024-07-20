using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ExtUnit5.Entities
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "Název nesmí být delší než 50 znaků.")]
        public string Name { get; set; }

        [AllowNull]
        [MaxLength(255)]
        [StringLength(255, ErrorMessage = "Description nesmí být delší než 255 znaků.")]
        public string? Description { get; set; }
        public virtual ICollection<Product> Products { get; set; } = null!;

        public override string ToString()
        {
            return Name;
        }
    }
}
