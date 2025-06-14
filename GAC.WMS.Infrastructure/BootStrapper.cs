using GAC.WMS.Domain.Repositories;
using GAC.WMS.Infrastructure.Data;
using GAC.WMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GAC.WMS.Infrastructure
{
    public static class BootStrapper
    {
        public static IServiceCollection Useinfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
          services.AddScoped<ICustomerRepository, CustomerRepository>();
          services.AddScoped<IProductRepository, ProductRepository>();
          services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
          services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
          services.AddDbContext<GacDbContext>(options =>
          {
              options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(60));
          });
          return services;
        }
    }
}
