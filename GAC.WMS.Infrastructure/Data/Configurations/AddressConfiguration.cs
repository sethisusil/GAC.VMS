using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GAC.WMS.Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address));
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Street).HasMaxLength(256).IsRequired();
            builder.Property(c => c.State).HasMaxLength(128).IsRequired();
            builder.Property(c => c.City).HasMaxLength(256).IsRequired();
            builder.Property(c => c.Country).HasMaxLength(128).IsRequired();
            builder.Property(c => c.ZipCode).HasMaxLength(10).IsRequired();
            builder.Property(c => c.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedBy).HasMaxLength(100).IsRequired().HasDefaultValue("[System]");
            builder.Property(c => c.UpdatedDate).IsRequired(false);
            builder.Property(c => c.UpdatedBy).IsRequired(false);
        }
    }
}
