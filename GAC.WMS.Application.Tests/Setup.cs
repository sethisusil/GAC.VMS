using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;
using GAC.WMS.Domain.Entities;

namespace GAC.WMS.Application.Tests
{
    public static class Setup
    {
        #region Customer
        public static CustomerDto GetGetCustomerDto(int id)
        {
            return new CustomerDto
            {
                Address = GetAddressdto(id),
                Id = id,
                AddressId = id,
                Name = $"Customer {id}",
                Email = $"testemail-{id}@gmail.com",
            };
        }
        public static CustomerRequest GetGetCustomerRequest(int id)
        {
            return new CustomerRequest
            {
                Address = GetAddressRequest(id),
                Name = $"Customer {id}",
                Email = $"testemail-{id}@gmail.com",
            };
        }
        public static Customer GetGetCustomerEntity(int id)
        {
            return new Customer
            {
                Address = GetAddressEntity(id),
                Email = $"testemail-{id}@gmail.com",
                Id = id,
                AddressId = id,
                Name = $"Customer {id}",
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region Address
        public static AddressDto GetAddressdto(int id)
        {
            return new AddressDto
            {
                Id = id,
                Street = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                Country = "Sample Country"
            };
        }
        public static AddressRequest GetAddressRequest(int id)
        {
            return new AddressRequest
            {
                Street = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                Country = "Sample Country"
            };
        }
        public static Address GetAddressEntity(int id)
        {
            return new Address
            {
                Id = id,
                Street = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                Country = "Sample Country",
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region Product
        public static ProductDto GetProductDto(int id)
        {
            return new ProductDto
            {
                Id = id,
                Title = $"Product {id}",
                Description = "Sample product description",
                Dimensions = GetDimensionsDto(id),
                Code = $"P{id:D3}",
                DimensionsId=id
            };
        }
        public static ProductRequest GetProductRequest(int id)
        {
            return new ProductRequest
            {
                Title = $"Product {id}",
                Description = "Sample product description",
                Dimensions = GetDimensionsRequest(id),
                Code = $"P{id:D3}"
            };
        }
        public static Product GetProductEntity(int id)
        {
            return new Product
            {
                Id = id,
                Title = $"Product {id}",
                Description = "Sample product description",
                Dimensions = GetDimensionsEntity(id),
                DimensionsId = id,
                Code = $"P{id:D3}",
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region Dimensions
        public static DimensionsDto GetDimensionsDto(int id)
        {
            return new DimensionsDto
            {
                Id = id,
                Length = 10.0m,
                Width = 5.0m,
                Height = 2.0m
            };
        }
        public static DimensionsRequest GetDimensionsRequest(int id)
        {
            return new DimensionsRequest
            {
                Length = 10.0m,
                Width = 5.0m,
                Height = 2.0m
            };
        }
        public static Dimensions GetDimensionsEntity(int id)
        {
            return new Dimensions
            {
                Id = id,
                Length = 10.0m,
                Width = 5.0m,
                Height = 2.0m,
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region PurchaseOrder
        public static PurchaseOrderDto GetPurchaseOrderDto(int id)
        {
            return new PurchaseOrderDto
            {
                Id = id,
                CustomerId = id,
                Customer = GetGetCustomerDto(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItemDto>
                {
                    GetOrderItemDto(id)
                }
            };
        }
        public static PurchaseOrderRequest GetPurchaseOrderRequest(int id)
        {
            return new PurchaseOrderRequest
            {
                CustomerId = id,
                Customer = GetGetCustomerRequest(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItemDto>
                {
                    GetOrderItemDto(id)
                }
            };
        }
        public static PurchaseOrder GetPurchaseOrderEntity(int id)
        {
            return new PurchaseOrder
            {
                Id = id,
                CustomerId = id,
                Customer = GetGetCustomerEntity(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItem>
                {
                    GetOrderItemEntity(id)
                },
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region OrderItem
        public static OrderItemDto GetOrderItemDto(int id)
        {
            return new OrderItemDto
            {
                ProductCode = $"P{id:D3}",
                Quantity=1
            };
        }
        public static OrderItem GetOrderItemEntity(int id)
        {
            return new OrderItem
            {
                Id = id,
                ProductId = id,
                Product = GetProductEntity(id),
                Quantity = 1,
                PurchaseOrderId=id,
                SalesOrderId = id,
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion

        #region SalesOrder
        public static SalesOrderDto GetSalesOrderDto(int id)
        {
            return new SalesOrderDto
            {
                Id = id,
                CustomerId = id,
                Customer = GetGetCustomerDto(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItemDto>
                {
                    GetOrderItemDto(id)
                }
            };
        }
        public static SalesOrderRequest GetSalesOrderRequest(int id)
        {
            return new SalesOrderRequest
            {
                CustomerId = id,
                Customer = GetGetCustomerRequest(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItemDto>
                {
                    GetOrderItemDto(id)
                }
            };
        }
        public static SalesOrder GetSalesOrderEntity(int id)
        {
            return new SalesOrder
            {
                Id = id,
                CustomerId = id,
                Customer = GetGetCustomerEntity(id),
                ProcessingDate = DateTime.UtcNow,
                Products = new List<OrderItem>
                {
                    GetOrderItemEntity(id)
                },
                CreatedBy = "TestUser",
                CreatedDate = DateTime.UtcNow
            };
        }
        #endregion
    }
}
