using GAC.WMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Data
{
    public class GacDbContext: DbContext
    {
        public GacDbContext(DbContextOptions<GacDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GacDbContext).Assembly);
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Dimensions> Dimensions => Set<Dimensions>();
    }
}
