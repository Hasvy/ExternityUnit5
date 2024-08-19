using Database;
using ExtUnit5.Components;
using ExtUnit5.Database;
using ExtUnit5.Security;
using ExtUnit5.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Quartz;
using ExtUnit5.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Configuration.AddJsonFile("appsettings.Local.json", true);

builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.AllowedForNewUsers = false;
    options.User.AllowedUserNameCharacters =
        @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+\/\\";
})
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IISDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<CodeGeneratorService>();
builder.Services.AddSingleton<FakeDataService>();
builder.Services.AddSingleton<AdjustProductsPricesJob>();
builder.Services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider<IdentityUser>>();

builder.Services.AddQuartz(q =>
{
    if (builder.Configuration.GetValue<bool>("BackgroundJobs:List:AdjustProductsPricesJob"))
    {
        var jobKey = nameof(AdjustProductsPricesJob);
        q.AddJob<AdjustProductsPricesJob>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithSimpleSchedule(o => o
                .RepeatForever()
                .WithIntervalInMinutes(1))
        );
    }
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    var seeder = new Seeder(scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>(),
                            scope.ServiceProvider.GetRequiredService<FakeDataService>(),
                            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
                            builder.Configuration);
    await seeder.SeedDatabase();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
