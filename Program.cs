using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Areas.Airlines.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AirBnBContext>(options =>
    options.UseSqlite("Data Source=grouponeflight.db"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite    = SameSiteMode.Lax;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AirBnBContext>();

    // Drop and recreate the database so the schema always matches the current models.
    // Safe for this project phase — no production data to preserve.
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    // Seed airlines
    context.Airlines.AddRange(
        new Airline { Name = "United Airlines",   ImageName = "united.png" },
        new Airline { Name = "American Airlines", ImageName = "american.png" },
        new Airline { Name = "Delta Air Lines",   ImageName = "delta.png" },
        new Airline { Name = "Southwest Airlines",ImageName = "southwest.png" },
        new Airline { Name = "JetBlue Airways",   ImageName = "jetblue.png" },
        new Airline { Name = "Alaska Airlines",   ImageName = "alaska.png" }
    );
    context.SaveChanges();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
