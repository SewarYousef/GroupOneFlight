using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.DataLayer;
using GroupOneFlight.Models.DataLayer.Repositories;
using GroupOneFlight.Models.DomainModels;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext - persistent SQLite path for Azure (/home) with local fallback
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SQLite")
        ?? "Data Source=/home/grouponeflight.db"));

// HTTP context accessor (for cookie helpers)
builder.Services.AddHttpContextAccessor();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite    = SameSiteMode.Lax;
});

// Repository registrations
builder.Services.AddScoped<IFlightRepository,        FlightRepository>();
builder.Services.AddScoped<IRepository<Airline>,     Repository<Airline>>();
builder.Services.AddScoped<IRepository<Reservation>, Repository<Reservation>>();
builder.Services.AddScoped<IRepository<FlightOptions>, Repository<FlightOptions>>();

var app = builder.Build();

// Ensure DB and schema are current
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

    // Wipe stale schema and recreate (safe — old path was ephemeral on Azure)
    // TODO: Comment out EnsureDeleted after first successful deploy to preserve data
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    // Seed data is now handled by AirlineConfig (DataLayer/Configuration)
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

// Area routing
app.MapControllerRoute(
    name:    "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default routing
app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
