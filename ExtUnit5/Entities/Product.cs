using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ExtUnit5.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Kategorie je povinná")]
        public virtual Category Category { get; set; } = null!;

        [AllowNull]
        [StringLength(255, ErrorMessage = "Description nesmí být delší než 255 znaků.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cena je povinná")]
        public float Price { get; set; } = 0f;


        [AllowNull]
        [Range(0, int.MaxValue, ErrorMessage = "Počet skladem musí být nezáporné číslo")]
        public int Stock { get; set; } = 0;

        [NotMapped]
        public bool IsActive => Stock > 0;

        public float AverageOrdered { get; set; }       //Per month

        public float Popularity
        {
            get => _popularity;
            set
            {
                _popularity = value;
                SetPopularityGroup();
            }
        }

        public float? LastPopularityAffectedPrice { get; set; } = null;

        public Group PopularityGroup { get; set; }

        //Variables
        private float _popularity = 1;

        //Methods
        public override string ToString()
        {
            return Name;
        }

        private void SetPopularityGroup()
        {
            if (Popularity > 1.1f)
                PopularityGroup = Group.Popular;
            else if (Popularity < 0.9f)
                PopularityGroup = Group.Unpopular;
            else
                PopularityGroup = Group.Neutral;
        }

        public enum Group
        {
            Popular,
            Neutral,
            Unpopular
        }
    }
}
