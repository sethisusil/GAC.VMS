using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
            builder.ToTable(nameof(SalesOrder));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.ProcessingDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CustomerId).IsRequired();
            builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.ShipmentAddressId).IsRequired();
            builder.HasOne(x => x.ShipmentAddress).WithMany().HasForeignKey(x => x.ShipmentAddressId);
            builder.HasMany(x => x.Products).WithOne().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
            builder.HasIndex(c => new { c.ProcessingDate, c.CustomerId }).IsUnique();
        }
    }
}
