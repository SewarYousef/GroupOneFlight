using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Areas.Airlines.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AirBnBContext>(options =>
    options.UseSqlite("Data Source=grouponeflight.db"));

// ✅ Session (you are using HttpContext.Session)
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ MUST be before MapControllerRoute
app.UseSession();

app.UseAuthorization();

// ✅ Areas routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// ✅ Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();