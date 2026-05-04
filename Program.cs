using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.DomainModels;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseSqlite("Data Source=grouponeflight.db"));

// Http context
builder.Services.AddHttpContextAccessor();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

    // // Recreate DB (DEV ONLY)
    // context.Database.EnsureDeleted();
    // context.Database.EnsureCreated();

    // Seed Airlines (ONLY if empty to avoid duplicates on restart)
    if (!context.Airlines.Any())
    {
        context.Airlines.AddRange(
            new Airline { Name = "United Airlines", ImageName = "united.png" },
            new Airline { Name = "American Airlines", ImageName = "american.png" },
            new Airline { Name = "Delta Air Lines", ImageName = "delta.png" },
            new Airline { Name = "Southwest Airlines", ImageName = "southwest.png" },
            new Airline { Name = "JetBlue Airways", ImageName = "jetblue.png" },
            new Airline { Name = "Alaska Airlines", ImageName = "alaska.png" }
        );

        context.SaveChanges();
    }
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

// Areas routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
