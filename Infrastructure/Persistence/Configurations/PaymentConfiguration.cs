using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.CompanyFeeAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.WorkerPayoutAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.PaymentStatus)
                .IsRequired();

            builder.HasIndex(x => x.BookingId)
                .IsUnique();

            builder.HasIndex(x => x.PaymentStatus);
            builder.HasIndex(x => x.PaidAtUtc);
        }
    }
}

