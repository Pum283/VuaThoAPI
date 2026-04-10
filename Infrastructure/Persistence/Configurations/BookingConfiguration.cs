using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BookingStatus)
                .IsRequired();

            builder.Property(x => x.ScheduledAtUtc)
                .IsRequired();

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.Property(x => x.EstimatedTotalAmount)
                .HasPrecision(18, 2);

            builder.HasOne<Domain.Entity.User>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<WorkerProfile>()
                .WithMany()
                .HasForeignKey(x => x.WorkerProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Payment)
                .WithOne(x => x.Booking)
                .HasForeignKey<Payment>(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Review)
                .WithOne(x => x.Booking)
                .HasForeignKey<Review>(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BookingStatus);
            builder.HasIndex(x => x.ScheduledAtUtc);
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.WorkerProfileId);
        }
    }
}

