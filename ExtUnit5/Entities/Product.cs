using System.Diagnostics.CodeAnalysis;

namespace ExtUnit5.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [AllowNull]
        public string Description { get; set; }
        public float Price { get; set; }
        [AllowNull]
        public int Stock { get; set; } = 0;
    }
}
