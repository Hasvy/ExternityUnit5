using Bogus;
using ExtUnit5.Entities;
using ExtUnit5.Helpers;

namespace ExtUnit5.Services
{
    public static class CategorySettings
    {
        public static int CategoryCount = 3;
        public static int ProductCount = 10;
        public static int CustomerCount = 50;
        public static int OrderCount = 200;
        //public static int OrderItemsCount = 50;
    }

    public class FakeDataService
    {
        public List<Category> CategoryList = new List<Category>();
        public List<Product> ProductList = new List<Product>();
        public List<Customer> CustomerList = new List<Customer>();
        public List<Order> OrderList = new List<Order>();
        public List<OrderItem> OrderItemsList = new List<OrderItem>();

        private readonly Faker<Category> _categoryFaker;
        private readonly Faker<Product> _productFaker;
        private readonly Faker<Customer> _customerFaker;
        private readonly Faker<Order> _orderFaker;
        private readonly Faker<OrderItem> _orderItemFaker;

        public FakeDataService()
        {
            _categoryFaker = new Faker<Category>("cz")
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1).First())
                .RuleFor(c => c.Description, f => f.Lorem.Slug(10));

            _productFaker = new Faker<Product>("cz")
                .RuleFor(p => p.Name, f => f.Commerce.Product())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Category, f => f.PickRandom(CategoryList))
                .RuleFor(p => p.Price, f => (float)Math.Round(f.Random.Float(0, 1000), 2))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 10));

            _customerFaker = new Faker<Customer>("cz")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.RegistrationDate, f => f.Date.Between(new DateTime(2021, 1, 1), DateTime.Today));

            _orderItemFaker = new Faker<OrderItem>("cz")
                .RuleFor(oi => oi.Quantity, f => f.Random.WeightedRandom([1, 2, 3, 4, 5], [0.50f, 0.20f, 0.15f, 0.10f, 0.5f]))
                .RuleFor(oi => oi.Product, f => f.PickRandom(ProductList));

            _orderFaker = new Faker<Order>("cz")
                .RuleFor(o => o.OrderItems, f => _orderItemFaker.Generate(f.Random.Int(1, 5)).ToList())
                .RuleFor(o => o.Customer, f => f.PickRandom(CustomerList))
                .RuleFor(o => o.OrderDate, f => GetWeightedDate(f))
                .RuleFor(o => o.Status, f => EnumHelper.GetRandomEnumValue<OrderStatus>());
        }

        //Returns rather weekends
        private DateTime GetWeightedDate(Faker f)
        {
            var dayOfweek = (DayOfWeek)f.Random.WeightedRandom([0, 1, 2, 3, 4, 5, 6], [0.2f, 0.12f, 0.12f, 0.12f, 0.12f, 0.12f, 0.2f]);
            if (dayOfweek == DayOfWeek.Sunday || dayOfweek == DayOfWeek.Saturday)
            {
                var randomDay = f.Date.Past(1);
                int daysToAdd = ((int)dayOfweek - (int)randomDay.DayOfWeek + 7) % 7;
                var a = randomDay.AddDays(daysToAdd);
                return a;
            }
            else
            {
                return f.Date.Past(1);
            }
        }

        public void Init()
        {
            CategoryList = _categoryFaker.Generate(CategorySettings.CategoryCount);
            ProductList = _productFaker.Generate(CategorySettings.ProductCount);
            CustomerList = _customerFaker.Generate(CategorySettings.CustomerCount);
            //OrderItemsList = _orderItemFaker.Generate(CategorySettings.OrderItemsCount);
            OrderList = _orderFaker.Generate(CategorySettings.OrderCount);
        }

        //public List<Category> GetCategories(int count)
        //{
        //    CategoryList = _categoryFaker.Generate(count);
        //    return CategoryList;
        //}

        //public List<Product> GetProducts(int count)
        //{
        //    ProductList = _productFaker.Generate(count);
        //    return ProductList;
        //}

        //public List<Customer> GetCustomers(int count)
        //{
        //    CustomerList = _customerFaker.Generate(count);
        //    return CustomerList;
        //}

        //public List<Order> GetOrders(int count)
        //{
        //    OrderList = _orderFaker.Generate(count);
        //    return OrderList;
        //}

        //public List<OrderItem> GetOrderItems(int count)
        //{
        //    OrderItemsList = _orderItemFaker.Generate(count);
        //    return OrderItemsList;
        //}
    }
}
