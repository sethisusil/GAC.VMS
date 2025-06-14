using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class DimensionsConfigurations : IEntityTypeConfiguration<Dimensions>
    {
        public void Configure(EntityTypeBuilder<Dimensions> builder)
        {
            builder.ToTable(nameof(Dimensions));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Length).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.Width).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.Height).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.Weight).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
        }
    }
}
