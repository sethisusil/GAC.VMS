using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(nameof(OrderItem));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.PurchaseOrderId).IsRequired(false);
            builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderId);
            builder.Property(c => c.SalesOrderId).IsRequired(false);
            builder.HasOne(x => x.SalesOrder).WithMany().HasForeignKey(x => x.SalesOrderId);
            builder.Property(c => c.ProductId).IsRequired();
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            builder.Property(c => c.Quantity).IsRequired().HasDefaultValue(0);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);

        }
    }
}
