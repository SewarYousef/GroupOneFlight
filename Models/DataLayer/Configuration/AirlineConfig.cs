using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.DataLayer.Configuration
{
    public class AirlineConfig : IEntityTypeConfiguration<Airline>
    {
        public void Configure(EntityTypeBuilder<Airline> builder)
        {
            builder.HasData(
                new Airline { Id = 1, Name = "United Airlines",    ImageName = "united.png"    },
                new Airline { Id = 2, Name = "American Airlines",  ImageName = "american.png"  },
                new Airline { Id = 3, Name = "Delta Air Lines",    ImageName = "delta.png"     },
                new Airline { Id = 4, Name = "Southwest Airlines", ImageName = "southwest.png" },
                new Airline { Id = 5, Name = "JetBlue Airways",    ImageName = "jetblue.png"   },
                new Airline { Id = 6, Name = "Alaska Airlines",    ImageName = "alaska.png"    }
            );
        }
    }
}
