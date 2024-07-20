using Bogus;
using ExtUnit5.Entities;

namespace ExtUnit5.Services
{
    public class FakeDataService
    {
        public List<Category> CategoryList;
        public List<Product> ProductList;
        public List<Customer> CustomerList;
        public List<Order> OrderList;
        public List<OrderItem> OrderItemsList;

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
                .RuleFor(p => p.Price, f => f.Random.Float(0, 1000))
                .RuleFor(p => p.Stock, f => f.Random.Int(0, 10));

            _customerFaker = new Faker<Customer>("cz")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.RegistrationDate, f => f.Date.Between(new DateTime(2010, 1, 1), DateTime.Today));

            _orderFaker = new Faker<Order>("cz")
                .RuleFor(o => o.Customer, f => f.PickRandom(CustomerList))
                .RuleFor(o => o.OrderDate, f => f.Date.Past(5))
                .RuleFor(o => o.Status, f => EnumHelper.GetRandomEnumValue<OrderStatus>())
                .RuleFor(o => o.TotalAmount, f => f.Random.Int(0, 50));

            _orderItemFaker = new Faker<OrderItem>("cz")
                .RuleFor(oi => oi.UnitPrice, f => f.Random.Decimal(1, 1000))
                .RuleFor(oi => oi.Quantity, f => f.Random.Int(0, 50))
                .RuleFor(oi => oi.Order, f => f.PickRandom(OrderList))
                .RuleFor(oi => oi.Product, f => f.PickRandom(ProductList));
        }

        public void Init(int count)
        {
            CategoryList = _categoryFaker.Generate(5);
            ProductList = _productFaker.Generate(count);
            CustomerList = _customerFaker.Generate(count);
            OrderList = _orderFaker.Generate(count);
            OrderItemsList = _orderItemFaker.Generate(count);
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
