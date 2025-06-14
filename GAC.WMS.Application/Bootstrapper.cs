using FluentValidation;
using FluentValidation.AspNetCore;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Application.Mappings;
using GAC.WMS.Application.Services;
using GAC.WMS.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace GAC.WMS.Application
{
    public static class Bootstrapper
    {
        public static IServiceCollection UserApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidatorsFromAssemblyContaining<CustomerRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<PurchaseOrderRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<SalesOrderRequestValidator>();
            services.AddFluentValidationAutoValidation();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ISalesOrderService, SalesOrderService>();
            return services;
        }
    }
}
