using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Code).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(156);
            builder.Property(c => c.Description).IsRequired().HasMaxLength(256);
            builder.Property(c => c.DimensionsId).IsRequired();
            builder.HasOne(x => x.Dimensions).WithMany().HasForeignKey(x => x.DimensionsId);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
