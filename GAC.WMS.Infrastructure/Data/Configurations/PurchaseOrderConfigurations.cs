using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class PurchaseOrderConfigurations : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable(nameof(PurchaseOrder));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.ProcessingDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CustomerId).IsRequired();
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            builder.HasMany(x => x.Products).WithOne().HasForeignKey(x => x.PurchaseOrderId);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
            builder.HasIndex(c => new { c.ProcessingDate, c.CustomerId }).IsUnique();
        }
    }
}
