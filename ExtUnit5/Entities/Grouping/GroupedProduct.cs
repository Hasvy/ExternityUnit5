namespace ExtUnit5.Entities.Grouping
{
    public class GroupedProduct
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public List<MonthlyOrder> DatesOrdered { get; set; }
    }

    public class MonthlyOrder
    {
        public DateTime Month { get; set; }
        public int OrderCount { get; set; }
    }
}
