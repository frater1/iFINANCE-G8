using Microsoft.EntityFrameworkCore;
using Group8_iFINANCE_APP.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC
builder.Services.AddControllersWithViews();

// 2) EF‐Core + MySQL
var conn = builder.Configuration.GetConnectionString("IFinanceConnection");
builder.Services.AddDbContext<Group8_iFINANCEAPP_DBContext>(options =>
    options.UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 21)))
           .LogTo(Console.WriteLine)
);

// 3) Session (in‐memory cookie session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.Cookie.HttpOnly = true;
    opts.IdleTimeout     = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// 4) HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// enable sessions **before** authorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();