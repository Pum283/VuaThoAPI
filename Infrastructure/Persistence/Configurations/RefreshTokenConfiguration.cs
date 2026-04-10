using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Salt)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CreatedIp)
                .HasMaxLength(100);

            builder.Property(x => x.RevokedIp)
                .HasMaxLength(100);

            builder.Property(x => x.RevokeReason)
                .HasMaxLength(500);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ExpiresAtUtc);
        }
    }
}

