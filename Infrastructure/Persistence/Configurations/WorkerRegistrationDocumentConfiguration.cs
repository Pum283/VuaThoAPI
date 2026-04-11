using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkerRegistrationDocumentConfiguration : IEntityTypeConfiguration<WorkerRegistrationDocument>
{
    public void Configure(EntityTypeBuilder<WorkerRegistrationDocument> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(x => x.StoredRelativePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(x => x.WorkerProfile)
            .WithMany(x => x.RegistrationDocuments)
            .HasForeignKey(x => x.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.WorkerProfileId);
    }
}
