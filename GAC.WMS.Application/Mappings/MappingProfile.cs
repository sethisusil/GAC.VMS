using AutoMapper;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;
using GAC.WMS.Domain.Entities;

namespace GAC.WMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<CustomerRequest, Customer>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<ProductRequest, Product>().ReverseMap();
            CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap();
            CreateMap<PurchaseOrderRequest, PurchaseOrder>().ReverseMap()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<SalesOrderDto, SalesOrder>().ReverseMap();
            CreateMap<SalesOrderRequest, SalesOrder>().ReverseMap()
              .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<DimensionsDto, Dimensions>().ReverseMap();
            CreateMap<DimensionsRequest, Dimensions>().ReverseMap();
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<AddressRequest, Address>().ReverseMap();
            CreateMap<OrderItemDto, OrderItem>().ReverseMap()
                    .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product!.Code));
        }
    }
}
