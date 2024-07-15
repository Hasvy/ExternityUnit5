using Bogus;
using ExtUnit5.Entities;

namespace ExtUnit5
{
    public class FakeDataService
    {
        public List<Customer> CustomerList;
        public List<Order> OrderList;

        private int custId = 1;
        private int orderId = 1;

        private readonly Faker<Customer> _customerFaker;
        private readonly Faker<Order> _orderFaker;

        public FakeDataService()
        {
            _customerFaker = new Faker<Customer>("cz")
                .RuleFor(u => u.Id, _ => custId++)
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.RegistrationDate, f => f.Date.Between(new DateTime(2010, 1, 1), DateTime.Today));

            _orderFaker = new Faker<Order>("cz")
                .RuleFor(o => o.Id, _ => orderId++)
                .RuleFor(o => o.Customer, f => f.PickRandom(CustomerList))
                .RuleFor(o => o.OrderDate, f => f.Date.Past(5))
                .RuleFor(o => o.Status, f => EnumHelper.GetRandomEnumValue<OrderStatus>())
                .RuleFor(o => o.TotalAmount, f => f.Random.Int(0, 50));
        }

        public List<Customer> GetCustomers(int count)
        {
            CustomerList = _customerFaker.Generate(count);
            return CustomerList;
        }

        public List<Order> GetOrders(int count)
        {
            OrderList = _orderFaker.Generate(count);
            return OrderList;
        }
    }
}
