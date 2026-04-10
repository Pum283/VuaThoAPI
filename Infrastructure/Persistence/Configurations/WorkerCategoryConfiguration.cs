using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkerCategoryConfiguration : IEntityTypeConfiguration<WorkerCategory>
    {
        public void Configure(EntityTypeBuilder<WorkerCategory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.WorkerProfileId, x.CategoryId })
                .IsUnique();
        }
    }
}

