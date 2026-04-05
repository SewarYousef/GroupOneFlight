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

// ✅ SEED DATA - Add initial airlines
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AirBnBContext>();
    context.Database.EnsureCreated();

    // Check if airlines already exist
    if (!context.Airlines.Any())
    {
        context.Airlines.AddRange(
            new Airline { Name = "United Airlines" },
            new Airline { Name = "American Airlines" },
            new Airline { Name = "Delta Air Lines" },
            new Airline { Name = "Southwest Airlines" },
            new Airline { Name = "JetBlue Airways" },
            new Airline { Name = "Alaska Airlines" }
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