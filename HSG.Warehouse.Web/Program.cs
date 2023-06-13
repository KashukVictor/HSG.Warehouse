using HSG.Warehouse.ClientServices;
using HSG.Warehouse.ClientServices.NbuCurrency;
using HSG.Warehouse.Interfaces;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Repository;
using HSG.Warehouse.Repository.Repository;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IOptionsRepository, OptionsRepository>();
builder.Services.AddScoped<ISeedData, SeedDataRepository>();
builder.Services.AddScoped<IInvoicesRepository, InvoicesRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordValidator, HSG.Warehouse.Web.Helpers.PasswordValidator>();
builder.Services.AddHttpClient<INbuCurrencyServiceClient, NbuCurrencyServiceClient>();

builder.Services.AddAuthentication("Cookie")
    .AddCookie("Cookie", config =>
    {
        config.LoginPath = "/Home/Login";
        config.AccessDeniedPath = "/Home/AccessDenied";
    });
builder.Services.AddAuthorization();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("HSG.Warehouse.Web"));
    //options.UseSqlServer(connection, b => b.MigrationsAssembly("HSG.Warehouse.Web"));
    //Не відслідковю запити
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
