using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkerProfileConfiguration : IEntityTypeConfiguration<WorkerProfile>
    {
        public void Configure(EntityTypeBuilder<WorkerProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkerType)
                .IsRequired();

            builder.Property(x => x.WorkerStatus)
                .IsRequired();

            builder.Property(x => x.Latitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.Longitude)
                .HasPrecision(9, 6);

            builder.Property(x => x.RatingAvg)
                .HasPrecision(3, 2);

            builder.Property(x => x.BasePrice)
                .HasPrecision(18, 2);

            builder.HasMany(x => x.WorkerCategories)
                .WithOne(x => x.WorkerProfile)
                .HasForeignKey(x => x.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.Latitude, x.Longitude });
            builder.HasIndex(x => x.RatingAvg);
            builder.HasIndex(x => x.WorkerStatus);
            builder.HasIndex(x => x.WorkerType);
        }
    }
}

