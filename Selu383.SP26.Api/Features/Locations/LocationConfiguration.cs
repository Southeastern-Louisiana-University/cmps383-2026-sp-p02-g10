using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Selu383.SP26.Api.Features.Locations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Address)
            .IsRequired();

        builder.Property(x => x.TableCount)
            .IsRequired();

        // Optional but nice: enforce minimum 1 at DB level too (SQL Server supports this)
        builder.HasCheckConstraint("CK_Locations_TableCount_Min1", "[TableCount] >= 1");
    }
}