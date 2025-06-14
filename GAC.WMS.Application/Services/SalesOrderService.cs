using AutoMapper;
using FluentValidation;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq.Expressions;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Services
{
    public class SalesOrderService: ISalesOrderService
    {
        public readonly ISalesOrderRepository _repository;
        public readonly IProductRepository _productRepository;
        public readonly ICustomerRepository _customerRepository;
        private readonly ILogger<SalesOrderService> _logger;
        private readonly IValidator<SalesOrderRequest> _validator;
        private readonly IMapper _mapper;

        public SalesOrderService(ISalesOrderRepository repository,
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            ILogger<SalesOrderService> logger,
            IValidator<SalesOrderRequest> validator,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<OperationResult<SalesOrderDto>> CreateAsync(SalesOrderRequest salesOrder)
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Initiating create SalesOrder with Request:{JsonSerializer.Serialize(salesOrder)}");
            OperationResult<SalesOrderDto> opResult = new OperationResult<SalesOrderDto> { Success = false };
            try
            {
                var validationResult = await _validator.ValidateAsync(salesOrder);
                if (!validationResult.IsValid)
                {
                    var error = string.Join(",", validationResult.Errors);
                    opResult.Message = error;
                    _logger.LogError($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Validation error ocurred while creating SalesOrder with Error:{error}");
                }
                else
                {
                    var customerId = salesOrder.CustomerId;
                    if (customerId <= 0 && salesOrder.Customer != null)
                    {
                        var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == salesOrder.Customer!.Email!.ToLower().Trim().ToLower());
                        if (customer != null)
                        {
                            customerId = customer.Id;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: CustomerId is provided as {customerId}");
                        var doesExist = await _customerRepository.GetByIdAsync(customerId);
                        if (doesExist == null)
                        {
                            customerId = 0;
                        }
                    }
                    if (customerId <= 0 && salesOrder.Customer != null)
                    {
                        _logger.LogError($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Validation error ocurred while creating purchase order with Error:CustomerId is required");
                        opResult.Message = "Please provide a valid customer";
                        return opResult;
                    }

                    var existingOrder = await _repository.GetFirstAsync(x => x.ProcessingDate == salesOrder.ProcessingDate && x.CustomerId == customerId,
                       p => p.Customer!, p => p.Products!);
                    if (existingOrder != null)
                    {
                        _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Order already exist in system with ProcessingDate:{salesOrder.ProcessingDate} and supplied customer.  So not creating again");
                        opResult.Message = $"Order already exist in system with ProcessingDate:{salesOrder.ProcessingDate} and supplied customer.  So not creating again";
                        opResult.Success = false;
                    }
                    else
                    {
                        var productCodes = salesOrder.Products?.Select(po => po.ProductCode) ?? Enumerable.Empty<string>().Distinct();
                        var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));
                        var newOrder = new SalesOrder
                        {
                            CreatedBy = "[System]",
                            CreatedDate = DateTime.UtcNow,
                            CustomerId = customerId,
                            ProcessingDate = salesOrder.ProcessingDate,
                            Products = await GetOrderProducts(salesOrder, products, 0),
                            ShipmentAddress = _mapper.Map<Address>(salesOrder.ShipmentAddress)
                        };
                        await _repository.AddAsync(newOrder);
                        opResult.Success = true;
                        opResult.Message = "Sales Order successfully created";
                        MapProducts(newOrder, products.ToList());
                        opResult.Data = _mapper.Map<SalesOrderDto>(newOrder);
                        _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Successfully created slaes order with Id:{newOrder.Id} for Request:{JsonSerializer.Serialize(salesOrder)}");
                    }
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: An un-expected error ocurred while creating sales order with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult<SalesOrderDto>> UpdateAsync(int id, SalesOrderRequest salesOrder)
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UpdateAsync)}: Initiating update sales order for id:{id} with Request:{JsonSerializer.Serialize(salesOrder)}");
            OperationResult<SalesOrderDto> opResult = new OperationResult<SalesOrderDto> { Success = false };
            try
            {
                if (id > 0)
                {
                    var validationResult = await _validator.ValidateAsync(salesOrder);
                    if (!validationResult.IsValid)
                    {
                        var error = string.Join(",", validationResult.Errors);
                        opResult.Message = error;
                        _logger.LogError($"{nameof(SalesOrderService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating sales order for Id:{id} with Error:{error}");
                    }
                    else
                    {
                        var customerId = salesOrder.CustomerId;
                        if (customerId <= 0 && salesOrder.Customer != null)
                        {
                            var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == salesOrder.Customer!.Email!.ToLower().Trim().ToLower());
                            if (customer != null)
                            {
                                customerId = customer.Id;
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(CreateAsync)}: CustomerId is provided as {customerId}");
                            var doesExist = await _customerRepository.GetByIdAsync(customerId);
                            if (doesExist == null)
                            {
                                customerId = 0;
                            }
                        }
                        if (customerId <= 0)
                        {
                            _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating purchase order for Id:{id} with Error:CustomerId is required");
                            opResult.Message = "Please provide a valid customer";
                            return opResult;
                        }

                        var productCodes = salesOrder.Products?.Select(po => po.ProductCode) ?? Enumerable.Empty<string>().Distinct();
                        var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));
                        
                        var existingOrder = await _repository.GetFirstAsync(x => x.Id == id,
                            p => p.Customer!, p => p.Products!);
                        if (existingOrder != null)
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Order already exist in system with ProcessingDate:{salesOrder.ProcessingDate}. So updating");
                            existingOrder.Products = await GetOrderProducts(salesOrder, products, existingOrder.Id);
                            existingOrder.CustomerId = customerId;
                            if (existingOrder.ShipmentAddress != null)
                            {
                                existingOrder.ShipmentAddress.Street = salesOrder.ShipmentAddress?.Street ?? existingOrder.ShipmentAddress.Street;
                                existingOrder.ShipmentAddress.City = salesOrder.ShipmentAddress?.City ?? existingOrder.ShipmentAddress.City;
                                existingOrder.ShipmentAddress.State = salesOrder.ShipmentAddress?.State ?? existingOrder.ShipmentAddress.State;
                                existingOrder.ShipmentAddress.ZipCode = salesOrder.ShipmentAddress?.ZipCode ?? existingOrder.ShipmentAddress.ZipCode;
                            }
                            else
                            {
                                existingOrder.ShipmentAddress = _mapper.Map<Address>(salesOrder.ShipmentAddress);
                                _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Shipment Address is null. So adding new Shipment Address");
                            } 
                            existingOrder.UpdatedDate = DateTime.UtcNow;
                            existingOrder.UpdatedBy= "[System]";
                            await _repository.UpdateAsync(existingOrder);
                            opResult.Success = true;
                            opResult.Message = "SalesOrder successfully updated";
                            MapProducts(existingOrder, products.ToList());
                            opResult.Data = _mapper.Map<SalesOrderDto>(existingOrder);
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Successfully updated SalesOrder with Id:{existingOrder.Id} for Request:{JsonSerializer.Serialize(salesOrder)}");
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Order dpesn't exist in system with ProcessingDate:{salesOrder.ProcessingDate}. So Adding");
                        }
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(SalesOrderService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating sales Order with Error:Id is required");
                    opResult.Message = "Id is required";
                    opResult.Success = false;
                }

            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(UpdateAsync)}: An un-expected error ocurred while updating sales Order for id:{id} customer with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UpdateAsync)}: Exit");
            return opResult;
        }

        public async Task<SalesOrderDto> GetAsync(int id)
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(GetAsync)}: Initiating Get Sales Order with id:{id}");
            SalesOrderDto data = null!;
            try
            {
                if (id > 0)
                {
                    var entity = await _repository.GetByIdAsync(id, x=>x.ShipmentAddress, x=>x.Customer.Address, x=>x.Products);
                    if (entity != null)
                    {
                        var productIds = entity.Products?.Select(po => po.ProductId) ?? Enumerable.Empty<int>();
                        var products = await _productRepository.GetWhereAsync(p => productIds.Contains(p.Id));
                        MapProducts(entity, products.ToList());
                        data = _mapper.Map<SalesOrderDto>(entity);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(SalesOrderService)}.{nameof(GetAsync)}: Record does not exist with Id:{id}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(SalesOrderService)}.{nameof(GetAsync)}: Validation error ocurred while fetching Sales Order with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(GetAsync)}: An un-expected error ocurred while fetching Sales Order for id:{id}");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(GetAsync)}: Exit");
            return data;
        }

        public async Task<IEnumerable<SalesOrderDto>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(GetAllAsync)}: Initiating Get all Sales Order");
            IEnumerable<SalesOrderDto> data = new List<SalesOrderDto>();
            try
            {
                var entities = await _repository.GetAllAsync(x => x.ShipmentAddress, x => x.Customer.Address, x => x.Products);
                if (entities?.Any() ?? false)
                {
                    var productIds = entities.SelectMany(po => po.Products?.Select(p => p.ProductId) ?? Enumerable.Empty<int>()).Distinct();
                    var products = await _productRepository.GetWhereAsync(p => productIds.Contains(p.Id));
                    foreach (var entity in entities)
                    {
                        MapProducts(entity, products.ToList());
                    }
                    data = _mapper.Map<List<SalesOrderDto>>(entities);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(GetAllAsync)}: An un-expected error ocurred while fetching Sales Order");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(GetAllAsync)}: Exit");
            return data;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(DeleteAsync)}: Initiating drop Sales Order with id:{id}");
            OperationResult opResult = new OperationResult();
            try
            {
                if (id > 0)
                {
                    await _repository.DeleteAsync(id);
                    opResult.Success = true;
                    opResult.Message = "Record successfully deleted";
                }
                else
                {
                    _logger.LogError($"{nameof(SalesOrderService)}.{nameof(DeleteAsync)}: Validation error ocurred while droping Sales Order with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(DeleteAsync)}: An un-expected error ocurred while deleting Sales Order for id:{id}");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(DeleteAsync)}: Exit");
            return opResult;
        }
        public async Task<OperationResult> UploadSalesOrdersAsync(IEnumerable<SalesOrderRequest> salesOrders)
        {
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Initiating upload Sales Orders with Request:{JsonSerializer.Serialize(salesOrders)}");
            OperationResult opResult = new OperationResult { Success = false };
            try
            {
                if (salesOrders?.Any() ?? false)
                {
                    var productCodes = salesOrders.SelectMany(po => po.Products?.Select(p => p.ProductCode) ?? Enumerable.Empty<string>()).Distinct();
                    var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));                    
                    foreach (var salesOrder in salesOrders)
                    {
                        var validationResult = await _validator.ValidateAsync(salesOrder);
                        if (validationResult?.Errors?.Any() ?? false)
                        {
                            validationResult.Errors.Remove(validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(SalesOrderDto.CustomerId))!); // Ignore CustomerId validation error if present
                        }
                        if (validationResult?.Errors?.Any() ?? false)
                        {
                            var error = string.Join(",", validationResult.Errors);
                            opResult.Message = error;
                            _logger.LogError($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Validation error ocurred while uploading Sales order with Error:{error}");
                        }
                        else
                        {
                            var customerId = salesOrder.CustomerId;
                            if (customerId <= 0 && salesOrder.Customer != null)
                            {
                                var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == salesOrder.Customer!.Email!.ToLower().Trim().ToLower());
                                if (customer != null)
                                {
                                    customerId = customer.Id;
                                }
                            }
                            Expression<Func<SalesOrder, bool>> query = c => c.ProcessingDate == salesOrder.ProcessingDate && c.Customer!.Name == salesOrder.Customer!.Name;
                            if (customerId > 0)
                            {
                                query = c => c.ProcessingDate == salesOrder.ProcessingDate && c.CustomerId == customerId;
                            }
                            var existingOrder = await _repository.GetFirstAsync(query,
                                p => p.Customer!, p => p.Products!);
                            if (existingOrder != null)
                            {
                                _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Order already exist in system with ProcessingDate:{salesOrder.ProcessingDate}. So updating");                                
                                if (salesOrder.ShipmentAddress != null)
                                {
                                    if (existingOrder.ShipmentAddress != null)
                                    {
                                        existingOrder.ShipmentAddress.Street = salesOrder.ShipmentAddress.Street!;
                                        existingOrder.ShipmentAddress.City = salesOrder.ShipmentAddress.City!;
                                        existingOrder.ShipmentAddress.State = salesOrder.ShipmentAddress.State!;
                                        existingOrder.ShipmentAddress.ZipCode = salesOrder.ShipmentAddress.ZipCode!;
                                    }
                                    else
                                    {
                                        existingOrder.ShipmentAddress = _mapper.Map<Address>(salesOrder.ShipmentAddress);
                                    }
                                }
                                existingOrder.Products = await GetOrderProducts(salesOrder, products, existingOrder.Id);
                                await _repository.UpdateAsync(existingOrder);
                            }
                            else
                            {
                                _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Order dpesn't exist in system with ProcessingDate:{salesOrder.ProcessingDate}. So Adding");

                                var newOrder = new SalesOrder
                                {
                                    CreatedBy = "[System]",
                                    CreatedDate = DateTime.UtcNow,
                                    CustomerId = customerId,
                                    Customer = customerId <= 0 ? _mapper.Map<Customer>(salesOrder.Customer) : null,
                                    ProcessingDate = salesOrder.ProcessingDate,
                                    Products = await GetOrderProducts(salesOrder, products, 0),
                                    ShipmentAddress = _mapper.Map<Address>(salesOrder.ShipmentAddress)
                                };
                                await _repository.AddAsync(newOrder);
                            }
                        }
                    }
                    opResult.Success = true;
                    opResult.Message = "Sales orders successfully uploaded";
                    _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Successfully uploaded Sales Orders with Request:{JsonSerializer.Serialize(salesOrders)}");
                }
                else
                {
                    _logger.LogError($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Validation error ocurred while uploading Sales Orders with Error:No Sales Orders provided");
                    opResult.Message = "No Sales Orders provided";
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: An un-expected error ocurred while uploading Sales Orders with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(SalesOrderService)}.{nameof(UploadSalesOrdersAsync)}: Exit");
            return opResult;
        }
        private async Task<List<OrderItem>> GetOrderProducts(SalesOrderRequest purchaseOrder, IEnumerable<Product> products, int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            if (purchaseOrder != null)
            {
                if (purchaseOrder.Products?.Any() ?? false)
                {
                    foreach (var product in purchaseOrder.Products)
                    {
                        var existingProduct = products.FirstOrDefault(p => p.Code == product.ProductCode);
                        if (existingProduct != null)
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadSalesOrdersAsync)}: Product already exist in system with Code:{product.ProductCode}. So Adding to Order");
                            orderItems.Add(new OrderItem
                            {
                                CreatedBy = "[System]",
                                CreatedDate = DateTime.UtcNow,
                                SalesOrderId = orderId,
                                ProductId = existingProduct.Id,
                                Quantity = product.Quantity
                            });
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadSalesOrdersAsync)}: Product dpesn't exist in system with Code:{product.ProductCode}.");                        
                        }
                    }
                }
            }
            return await Task.FromResult(orderItems);
        }
        private void MapProducts(SalesOrder purchaseOrder, List<Product> product)
        {
            if (purchaseOrder.Products?.Any() ?? false)
            {
                foreach (var item in purchaseOrder.Products)
                {
                    item.Product = product.FirstOrDefault(p => p.Id == item.ProductId);
                }
            }
        }
    }
}
