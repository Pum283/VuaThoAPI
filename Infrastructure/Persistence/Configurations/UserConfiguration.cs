using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.UserName)
                .HasMaxLength(256);

            builder.Property(x => x.NormalizedUserName)
                .HasMaxLength(256);

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Email)
                .HasMaxLength(256);

            builder.Property(x => x.NormalizedEmail)
                .HasMaxLength(256);

            builder.HasIndex(x => x.PhoneNumber)
                .IsUnique();

            builder.HasIndex(x => x.Email);

            builder.HasOne(x => x.WorkerProfile)
                .WithOne(x => x.User)
                .HasForeignKey<WorkerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Users");
        }
    }
}

