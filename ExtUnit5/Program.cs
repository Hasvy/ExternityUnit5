using Bogus;
using Database;
using ExtUnit5;
using ExtUnit5.Components;
using ExtUnit5.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSingleton<FakeDataService>();

var app = builder.Build();

List<Category> categories = new List<Category>
{
    new Category { Name = "Category 1" },
    new Category { Name = "Category 2" },
    new Category { Name = "Category 3" },
};

List<Product> products = new List<Product>
{
    new Product { Category = categories[0], Name = "Product 1", Description = "Description 1", Price = 10.99f, Stock = 5 },
    new Product { Category = categories[0], Name = "Product 2", Description = "Description 2", Price = 12.99f, Stock = 0 },
    new Product { Category = categories[1], Name = "Product 3", Description = "Description 3", Price = 8.99f, Stock = 15 },
    new Product { Category = categories[1], Name = "Product 4", Description = "Description 4", Price = 7.49f, Stock = 30 },
    new Product { Category = categories[1], Name = "Product 5", Description = "Description 5", Price = 20.00f, Stock = 0 },
    new Product { Category = categories[0], Name = "Product 6", Description = "Description 6", Price = 25.50f, Stock = 10 },
    new Product { Category = categories[0], Name = "Product 7", Description = "Description 7", Price = 5.99f, Stock = 50 },
    new Product { Category = categories[2], Name = "Product 8", Description = "Description 8", Price = 15.75f, Stock = 8 },
    new Product { Category = categories[2], Name = "Product 9", Description = "Description 9", Price = 11.30f, Stock = 0 },
    new Product { Category = categories[2], Name = "Product 10", Description = "Description 10", Price = 9.99f, Stock = 22 }
};

List<Customer> customers = new List<Customer>
{
    new Customer { FirstName = "Abc", LastName = "Afef", Email = "fwefwe", RegistrationDate = DateTime.Now }
};

List<Order> orders = new List<Order>
{
    new Order { Customer = customers[0], Status = OrderStatus.New, TotalAmount = 10, OrderDate = DateTime.Now }
};

List<OrderItem> orderItems = new List<OrderItem>
{
    new OrderItem { Order = orders[0], Product = products[3], Quantity = 10, UnitPrice = 10.50m }
};

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.Products.AddRange(products);
    context.Categories.AddRange(categories);
    context.Customers.AddRange(customers);
    context.Orders.AddRange(orders);
    context.OrderItems.AddRange(orderItems);

    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
