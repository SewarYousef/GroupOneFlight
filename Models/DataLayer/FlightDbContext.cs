using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.DomainModels;
using GroupOneFlight.Models.ExtensionMethods;

namespace GroupOneFlight.Models.DataLayer
{
    public class FlightDbContext : DbContext
    {
        public FlightDbContext(DbContextOptions<FlightDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<FlightOptions> FlightOptions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Fluent API configurations go here

            // Example (only if needed):
            // modelBuilder.Entity<Flight>()
            //     .HasOne(f => f.Airline)
            //     .WithMany(a => a.Flights)
            //     .HasForeignKey(f => f.AirlineId);
        }
    }
}
