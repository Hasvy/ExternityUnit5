using Database;
using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExtUnit5.Components.Pages.CompareProducts
{
    public partial class CompareProducts : ComponentBase
    {
        [Inject] IDbContextFactory<AppDbContext> DbContextFactory { get; set; } = null!;
        private AppDbContext AppDbContext { get; set; } = null!;
        public Product? LeftProduct { get; set; }
        public Product? RightProduct { get; set; }
        public float PriceDiff { get; set; }
        public float AverageOrderedDiff { get; set; }
        public float PopularityDiff { get; set; }

        protected override Task OnInitializedAsync()
        {
            AppDbContext = DbContextFactory.CreateDbContext();
            return base.OnInitializedAsync();
        }

        private void SetLeftProduct(Product leftProduct)
        {
            LeftProduct = leftProduct;
            if (RightProduct is not null)
                SetDifferences(LeftProduct, RightProduct);
        }

        private void SetRightProduct(Product rightProduct)
        {
            RightProduct = rightProduct;
            if (LeftProduct is not null)
                SetDifferences(LeftProduct, RightProduct);
        }

        private void SetDifferences(Product left, Product right)
        {
            PriceDiff = (float)Math.Round(left.Price - right.Price, 2);
            AverageOrderedDiff = (float)Math.Round(left.AverageOrdered - right.AverageOrdered, 2);
            PopularityDiff = (float)Math.Round(left.Popularity - right.Popularity, 2);
        }
    }
}
