using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable(nameof(Customer));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(100).IsRequired();
            builder.Property(c => c.AddressId).IsRequired();
            builder.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId);
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
            builder.HasIndex(c => c.Email).IsUnique();
        }
    }
}
