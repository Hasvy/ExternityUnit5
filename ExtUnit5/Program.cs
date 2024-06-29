using Database;
using ExtUnit5.Components;
using ExtUnit5.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

List<Product> products = new List<Product>
{
    new Product { Name = "Product 1", Description = "Description 1", Price = 10.99f, Stock = 5 },
    new Product { Name = "Product 2", Description = "Description 2", Price = 12.99f, Stock = 0 },
    new Product { Name = "Product 3", Description = "Description 3", Price = 8.99f, Stock = 15 },
    new Product { Name = "Product 4", Description = "Description 4", Price = 7.49f, Stock = 30 },
    new Product { Name = "Product 5", Description = "Description 5", Price = 20.00f, Stock = 0 },
    new Product { Name = "Product 6", Description = "Description 6", Price = 25.50f, Stock = 10 },
    new Product { Name = "Product 7", Description = "Description 7", Price = 5.99f, Stock = 50 },
    new Product { Name = "Product 8", Description = "Description 8", Price = 15.75f, Stock = 8 },
    new Product { Name = "Product 9", Description = "Description 9", Price = 11.30f, Stock = 0 },
    new Product { Name = "Product 10", Description = "Description 10", Price = 9.99f, Stock = 22 }
};

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    context.Products.AddRange(products);
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
