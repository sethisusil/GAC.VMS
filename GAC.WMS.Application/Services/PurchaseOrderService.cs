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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        public readonly IPurchaseOrderRepository _repository;
        public readonly IProductRepository _productRepository;
        public readonly ICustomerRepository _customerRepository;
        private readonly ILogger<PurchaseOrderService> _logger;
        private readonly IValidator<PurchaseOrderRequest> _validator;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repository,
            IProductRepository productRepository,
            ILogger<PurchaseOrderService> logger,
            ICustomerRepository customerRepository,
            IValidator<PurchaseOrderRequest> validator,
            IMapper mapper)
        {
            _repository = repository;
            _productRepository = productRepository;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<OperationResult<PurchaseOrderDto>> CreateAsync(PurchaseOrderRequest purchaseOrder)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Initiating create purchase order with Request:{JsonSerializer.Serialize(purchaseOrder)}");
            OperationResult<PurchaseOrderDto> opResult = new OperationResult<PurchaseOrderDto> { Success = false };
            try
            {
                var validationResult = await _validator.ValidateAsync(purchaseOrder);
                if (!validationResult.IsValid)
                {
                    var error = string.Join(",", validationResult.Errors);
                    opResult.Message = error;
                    _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Validation error ocurred while creating purchase order with Error:{error}");
                }
                else
                {
                    var customerId = purchaseOrder.CustomerId ?? 0;
                    if (customerId <= 0 && purchaseOrder.Customer != null)
                    {
                        var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == purchaseOrder.Customer!.Email!.ToLower().Trim().ToLower());
                        if (customer != null)
                        {
                            customerId = customer.Id;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: CustomerId is provided as {customerId}");
                        var doesExist = await _customerRepository.GetFirstAsync(c => c.Id == customerId);
                        if (doesExist == null)
                        {
                            customerId = 0;
                        }
                    }
                    if (customerId <= 0 && purchaseOrder.Customer != null)
                    {
                        _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Validation error ocurred while creating purchase order with Error:CustomerId is required");
                        opResult.Message = "Please provide a valid customer";
                        return opResult;
                    }

                    var existingOrder = await _repository.GetFirstAsync(x => x.ProcessingDate == purchaseOrder.ProcessingDate && x.CustomerId == customerId,
                        p => p.Customer!, p => p.Products!);
                    if (existingOrder != null)
                    {
                        _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Order already exist in system with ProcessingDate:{purchaseOrder.ProcessingDate} and supplied customer.  So not creating again");
                        opResult.Message = $"Order already exist in system with ProcessingDate:{purchaseOrder.ProcessingDate} and supplied customer.  So not creating again";
                        opResult.Success = false;
                    }
                    else
                    {
                        var productCodes = purchaseOrder.Products?.Select(po => po.ProductCode) ?? Enumerable.Empty<string>().Distinct();
                        var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));
                        var newOrder = new PurchaseOrder
                        {
                            CreatedBy = "[System]",
                            CreatedDate = DateTime.UtcNow,
                            CustomerId = customerId,
                            ProcessingDate = purchaseOrder.ProcessingDate ?? DateTime.UtcNow,
                            Products = await GetOrderProducts(purchaseOrder, products, 0)
                        };
                        await _repository.AddAsync(newOrder);
                        opResult.Success = true;
                        opResult.Message = "Purchase Order successfully created";                       
                        MapProducts(newOrder, products.ToList());
                        opResult.Data = _mapper.Map<PurchaseOrderDto>(newOrder);
                        _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Successfully created purchase order with Id:{newOrder.Id} for Request:{JsonSerializer.Serialize(purchaseOrder)}");
                    }
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: An un-expected error ocurred while creating purchase order with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult<PurchaseOrderDto>> UpdateAsync(int id, PurchaseOrderRequest purchaseOrder)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Initiating update purchase order for id:{id} with Request:{JsonSerializer.Serialize(purchaseOrder)}");
            OperationResult<PurchaseOrderDto> opResult = new OperationResult<PurchaseOrderDto> { Success = false };
            try
            {
                if (id > 0)
                {
                    var validationResult = await _validator.ValidateAsync(purchaseOrder);
                    if (!validationResult.IsValid)
                    {
                        var error = string.Join(",", validationResult.Errors);
                        opResult.Message = error;
                        _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating purchase order for Id:{id} with Error:{error}");
                    }
                    else
                    {
                        var customerId = purchaseOrder.CustomerId ?? 0;
                        if (customerId <= 0 && purchaseOrder.Customer != null)
                        {
                            var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == purchaseOrder.Customer!.Email!.ToLower().Trim().ToLower());
                            if (customer != null)
                            {
                                customerId = customer.Id;
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(CreateAsync)}: CustomerId is provided as {customerId}");
                            var doesExist = await _customerRepository.GetFirstAsync(c => c.Id == customerId);
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
                        var productCodes = purchaseOrder.Products?.Select(po => po.ProductCode) ?? Enumerable.Empty<string>().Distinct();
                        var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));

                        var existingOrder = await _repository.GetFirstAsync(x => x.Id == id,
                            p => p.Customer!, p => p.Products!);
                        if (existingOrder != null)
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Order already exist in system with ProcessingDate:{purchaseOrder.ProcessingDate}. So updating");
                            existingOrder.Products = await GetOrderProducts(purchaseOrder, products, existingOrder.Id);
                            existingOrder.CustomerId = customerId;
                            existingOrder.UpdatedDate = DateTime.UtcNow;
                            existingOrder.UpdatedBy = "[System]";
                            await _repository.UpdateAsync(existingOrder);
                            opResult.Success = true;
                            opResult.Message = "PurchaseOrder successfully updated";
                            MapProducts(existingOrder, products.ToList());
                            opResult.Data = _mapper.Map<PurchaseOrderDto>(existingOrder);
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Successfully updated PurchaseOrder with Id:{existingOrder.Id} for Request:{JsonSerializer.Serialize(purchaseOrder)}");
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Order dpesn't exist in system with ProcessingDate:{purchaseOrder.ProcessingDate}. So Adding");
                        }

                    }
                }
                else
                {
                    _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating PurchaseOrder with Error:Id is required");
                    opResult.Message = "Id is required";
                    opResult.Success = false;
                }

            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: An un-expected error ocurred while updating PurchaseOrder for id:{id} customer with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UpdateAsync)}: Exit");
            return opResult;
        }

        public async Task<PurchaseOrderDto> GetAsync(int id)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(GetAsync)}: Initiating Get PurchaseOrder with id:{id}");
            PurchaseOrderDto data = null!;
            try
            {
                if (id > 0)
                {
                    var entity = await _repository.GetByIdAsync(id, x => x.Customer.Address, x => x.Products);
                    if (entity != null)
                    {
                        var productIds = entity.Products?.Select(po => po.ProductId) ?? Enumerable.Empty<int>();
                        var products = await _productRepository.GetWhereAsync(p => productIds.Contains(p.Id));
                        MapProducts(entity, products.ToList());
                        data = _mapper.Map<PurchaseOrderDto>(entity);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(GetAsync)}: Record does not exist with Id:{id}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(GetAsync)}: Validation error ocurred while fetching PurchaseOrder with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(GetAsync)}: An un-expected error ocurred while fetching PurchaseOrder for id:{id}");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(GetAsync)}: Exit");
            return data;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(GetAllAsync)}: Initiating Get all PurchaseOrder");
            IEnumerable<PurchaseOrderDto> data = new List<PurchaseOrderDto>();
            try
            {
                var entities = await _repository.GetAllAsync(x => x.Customer.Address, x => x.Products);
                if (entities?.Any() ?? false)
                {
                    var productIds = entities.SelectMany(po => po.Products?.Select(p => p.ProductId) ?? Enumerable.Empty<int>()).Distinct();
                    var products = await _productRepository.GetWhereAsync(p => productIds.Contains(p.Id));
                    foreach (var entity in entities)
                    {
                        MapProducts(entity, products.ToList());
                    }
                    data = _mapper.Map<List<PurchaseOrderDto>>(entities);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(GetAllAsync)}: An un-expected error ocurred while fetching PurchaseOrder");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(GetAllAsync)}: Exit");
            return data;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(DeleteAsync)}: Initiating drop PurchaseOrder with id:{id}");
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
                    _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(DeleteAsync)}: Validation error ocurred while droping PurchaseOrder with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(DeleteAsync)}: An un-expected error ocurred while deleting PurchaseOrder for id:{id}");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(DeleteAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult> UploadPurchaseOrdersAsync(IEnumerable<PurchaseOrderRequest> purchaseOrders)
        {
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Initiating upload purchase orders with Request:{JsonSerializer.Serialize(purchaseOrders)}");
            OperationResult opResult = new OperationResult { Success = false };
            try
            {
                if (purchaseOrders?.Any() ?? false)
                {
                    var productCodes = purchaseOrders.SelectMany(po => po.Products?.Select(p => p.ProductCode) ?? Enumerable.Empty<string>()).Distinct();
                    var products = await _productRepository.GetWhereAsync(p => productCodes.Contains(p.Code));
                    foreach (var purchaseOrder in purchaseOrders)
                    {
                        var validationResult = await _validator.ValidateAsync(purchaseOrder);
                        if (validationResult?.Errors?.Any() ?? false)
                        {
                            validationResult.Errors.Remove(validationResult.Errors.FirstOrDefault(e => e.PropertyName == nameof(PurchaseOrderDto.CustomerId))!); // Ignore CustomerId validation error if present
                        }
                        if (validationResult?.Errors?.Any() ?? false)
                        {
                            var error = string.Join(",", validationResult.Errors);
                            opResult.Message = error;
                            _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Validation error ocurred while uploading purchase order with Error:{error}");
                        }
                        else
                        {
                            var customerId = purchaseOrder.CustomerId ?? 0;
                            if (customerId <= 0 && purchaseOrder.Customer != null)
                            {
                                var customer = await _customerRepository.GetFirstAsync(c => c.Email.Trim().ToLower() == purchaseOrder.Customer!.Email!.ToLower().Trim().ToLower());
                                if (customer != null)
                                {
                                    customerId = customer.Id;
                                }
                            }

                            Expression<Func<PurchaseOrder, bool>> query = c => c.ProcessingDate == purchaseOrder.ProcessingDate && c.Customer!.Email == purchaseOrder.Customer!.Email;
                            if (customerId > 0)
                            {
                                query = c => c.ProcessingDate == purchaseOrder.ProcessingDate && c.CustomerId == customerId;
                            }
                            var existingOrder = await _repository.GetFirstAsync(query,
                                p => p.Customer!, p => p.Products!);
                            if (existingOrder != null)
                            {
                                _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Order already exist in system with ProcessingDate:{purchaseOrder.ProcessingDate}. So updating");
                                existingOrder.Products = await GetOrderProducts(purchaseOrder, products, existingOrder.Id);
                                await _repository.UpdateAsync(existingOrder);
                            }
                            else
                            {
                                _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Order dpesn't exist in system with ProcessingDate:{purchaseOrder.ProcessingDate}. So Adding");

                                var newOrder = new PurchaseOrder
                                {
                                    CreatedBy = "[System]",
                                    CreatedDate = DateTime.UtcNow,
                                    CustomerId = customerId,
                                    Customer = customerId <= 0 ? _mapper.Map<Customer>(purchaseOrder.Customer) : null,
                                    ProcessingDate = purchaseOrder.ProcessingDate ?? DateTime.UtcNow,
                                    Products = await GetOrderProducts(purchaseOrder, products, 0)
                                };
                                await _repository.AddAsync(newOrder);
                            }
                        }
                    }
                    opResult.Success = true;
                    opResult.Message = "Purchase orders successfully uploaded";
                    _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Successfully uploaded purchase orders with Request:{JsonSerializer.Serialize(purchaseOrders)}");
                }
                else
                {
                    _logger.LogError($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Validation error ocurred while uploading Purchase Orders with Error:Request is empty");
                    opResult.Message = "Request is empty";
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: An un-expected error ocurred while uploading Purchase Orders with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Exit");
            return opResult;
        }

        private async Task<List<OrderItem>> GetOrderProducts(PurchaseOrderRequest purchaseOrder, IEnumerable<Product> products, int orderId)
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
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Product already exist in system with Code:{product.ProductCode}. So Adding to Order");
                            orderItems.Add(new OrderItem
                            {
                                CreatedBy = "[System]",
                                CreatedDate = DateTime.UtcNow,
                                PurchaseOrderId = orderId,
                                ProductId = existingProduct.Id,
                                Quantity = product.Quantity
                            });
                        }
                        else
                        {
                            _logger.LogInformation($"{nameof(PurchaseOrderService)}.{nameof(UploadPurchaseOrdersAsync)}: Product dpesn't exist in system with Code:{product.ProductCode}.");
                        }
                    }
                }
            }
            return await Task.FromResult(orderItems);
        }
        private void MapProducts(PurchaseOrder purchaseOrder, List<Product> product)
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
