using Microsoft.EntityFrameworkCore;
using GroupOneFlight.Models.DomainModels;
using GroupOneFlight.Models.DataLayer.Configuration;

namespace GroupOneFlight.Models.DataLayer
{
    public class FlightDbContext : DbContext
    {
        public FlightDbContext(DbContextOptions<FlightDbContext> options)
            : base(options) { }

        public DbSet<Flight>       Flights      { get; set; }
        public DbSet<Airline>      Airlines     { get; set; }
        public DbSet<FlightOptions> FlightOptions { get; set; }
        public DbSet<Reservation>  Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply seed data from Configuration folder
            modelBuilder.ApplyConfiguration(new AirlineConfig());
        }
    }
}
