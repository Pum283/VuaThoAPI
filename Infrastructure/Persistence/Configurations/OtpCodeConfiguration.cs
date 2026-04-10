using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
    {
        public void Configure(EntityTypeBuilder<OtpCode> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(256);

            builder.Property(x => x.CodeHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Salt)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.VerificationToken)
                .HasMaxLength(200);

            builder.HasIndex(x => new { x.PhoneNumber, x.ExpiresAtUtc });
            builder.HasIndex(x => new { x.Email, x.ExpiresAtUtc });
            builder.HasIndex(x => x.VerificationToken);
        }
    }
}

