using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProdajnikWeb.Data;
using ProdajnikWeb.Models;
using ProdajnikWeb.Data.Initializer;
using Microsoft.AspNetCore.Authorization;
using ProdajnikWeb.Service;
using ProdajnikWeb.Data;
using ProdajnikWeb.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();


builder.Services.AddSession();

builder.Services.AddDbContext<ProdajnikContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

// ����������� ApplicationDbContext ��� Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // ���� ��� ��������������� �� �������� �����
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // ���� ��� �������� � ������������ �������
});


builder.Services.AddControllersWithViews(options =>
{
    // ��������� ���������� �������� �����������
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax; // �������� ��������� cookies
});
///////////////////////////////////
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// ��������� middleware
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // ������ ������������

// ������������� ���� ������
app.UseDbInitializer();

// ��������� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
