using Microsoft.EntityFrameworkCore;

namespace GroupOneFlight.Areas.Airlines.Models
{
    public class AirBnBContext : DbContext
    {
        public AirBnBContext(DbContextOptions<AirBnBContext> options)
            : base(options)
        {
        }

        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<Airline> Airlines => Set<Airline>();
        public DbSet<FlightOptions> FlightOptions => Set<FlightOptions>();
    }
}