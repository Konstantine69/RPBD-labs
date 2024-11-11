using Microsoft.EntityFrameworkCore;
using ProdajnikWebController.Data;
using ProdajnikWebController.Service;
using ProdajnikWebController.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();


builder.Services.AddSession();

builder.Services.AddDbContext<ProdajnikContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseDbInitializer();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();