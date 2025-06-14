using AutoMapper;
using FluentValidation;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Services
{
    public class ProductService: IProductService
    {
        public readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;
        private readonly IValidator<ProductRequest> _validator;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository,
            ILogger<ProductService> logger,
            IValidator<ProductRequest> validator,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<OperationResult<ProductDto>> CreateAsync(ProductRequest product)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(CreateAsync)}: Initiating create Product with Request:{JsonSerializer.Serialize(product)}");
            OperationResult<ProductDto> opResult = new OperationResult<ProductDto> { Success = false };
            try
            {
                var validationResult = await _validator.ValidateAsync(product);
                if (!validationResult.IsValid)
                {
                    var error = string.Join(",", validationResult.Errors);
                    opResult.Message = error;
                    _logger.LogError($"{nameof(ProductService)}.{nameof(CreateAsync)}: Validation error ocurred while creating product with Error:{error}");
                }
                else
                {
                    var existingProduct = await _repository.GetFirstAsync(c => c.Code.ToLower() == product.Code!.ToLower());
                    if (existingProduct != null)
                    {
                        _logger.LogError($"{nameof(ProductService)}.{nameof(CreateAsync)}: Product already exist in system with Code:{product.Code}. So not creating");
                        opResult.Message = $"Product with Code:{product.Code} already exists";
                        return opResult;
                    }
                    var entity = _mapper.Map<Product>(product);
                    await _repository.AddAsync(entity);
                    opResult.Success = true;
                    opResult.Message = "Product successfully created";
                    opResult.Data = _mapper.Map<ProductDto>(entity);
                    _logger.LogInformation($"{nameof(ProductService)}.{nameof(CreateAsync)}: Successfully created product with Id:{entity.Id} for Request:{JsonSerializer.Serialize(product)}");
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(CreateAsync)}: An un-expected error ocurred while creating product with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(CreateAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult<ProductDto>> UpdateAsync(int id, ProductRequest product)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Initiating update product for id:{id} with Request:{JsonSerializer.Serialize(product)}");
            OperationResult<ProductDto> opResult = new OperationResult<ProductDto> { Success = false };
            try
            {
                if (id > 0)
                {
                    var validationResult = await _validator.ValidateAsync(product);
                    if (!validationResult.IsValid)
                    {
                        var error = string.Join(",", validationResult.Errors);
                        opResult.Message = error;
                        _logger.LogError($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating customer for Id:{id} with Error:{error}");
                    }
                    else
                    {
                        var existingProduct = await _repository.GetByIdAsync(id, p => p.Dimensions);
                        if (existingProduct == null)
                        {
                            _logger.LogError($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Record does not exist with Id:{id}");
                            opResult.Message = $"Record does not exist with Id:{id}";
                            return opResult;
                        }
                        var duplicateCode = await _repository.GetFirstAsync(c => c.Code.ToLower() == product.Code!.ToLower() && c.Id != id);
                        if (duplicateCode != null)
                        {
                            _logger.LogError($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Product already exist in system with Code:{product.Code}. So not updating");
                            opResult.Message = $"Product with Code:{product.Code} already exists";
                            return opResult;
                        }
                        existingProduct.Code = product.Code!;
                        existingProduct.Title = product.Title!;
                        existingProduct.Description = product.Description!;
                        existingProduct.UpdatedDate = DateTime.UtcNow;
                        existingProduct.UpdatedBy = "[System]";
                        if (product.Dimensions != null)
                        {
                            if (existingProduct.Dimensions != null)
                            {
                                existingProduct.Dimensions.Length = product.Dimensions.Length;
                                existingProduct.Dimensions.Width = product.Dimensions.Width;
                                existingProduct.Dimensions.Weight = product.Dimensions.Weight;
                                existingProduct.Dimensions.Height = product.Dimensions.Height;
                            }
                            else
                            {
                                existingProduct.Dimensions = _mapper.Map<Dimensions>(product.Dimensions);
                            }
                        }
                        await _repository.UpdateAsync(existingProduct);
                        opResult.Success = true;
                        opResult.Message = "Product successfully updated";
                        opResult.Data = _mapper.Map<ProductDto>(existingProduct);
                        _logger.LogInformation($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Successfully updated product with customerId:{existingProduct.Id} for Request:{JsonSerializer.Serialize(product)}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating product with Error:Id is required");
                    opResult.Message = "Id is required";
                    opResult.Success = false;
                }

            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(UpdateAsync)}: An un-expected error ocurred while updating product for id:{id} customer with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(UpdateAsync)}: Exit");
            return opResult;
        }

        public async Task<ProductDto> GetAsync(int id)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetAsync)}: Initiating Get product with id:{id}");
            ProductDto data = null!;
            try
            {
                if (id > 0)
                {
                    var entity = await _repository.GetByIdAsync(id, p=>p.Dimensions);
                    if (entity != null)
                    {
                        data = _mapper.Map<ProductDto>(entity);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(ProductService)}.{nameof(GetAsync)}: Record does not exist with Id:{id}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(ProductService)}.{nameof(GetAsync)}: Validation error ocurred while fetching product with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(GetAsync)}: An un-expected error ocurred while fetching product for id:{id}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetAsync)}: Exit");
            return data;
        }

        public async Task<ProductDto> GetByCodeAsync(string code)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetByCodeAsync)}: Initiating Get product with code:{code}");
            ProductDto data = null!;
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var entity = await _repository.GetByCodeAsync(code, p => p.Dimensions);
                    if (entity != null)
                    {
                        data = _mapper.Map<ProductDto>(entity);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(ProductService)}.{nameof(GetByCodeAsync)}: Record does not exist with code:{code}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(ProductService)}.{nameof(GetByCodeAsync)}: Validation error ocurred while fetching product with Error:code is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(GetByCodeAsync)}: An un-expected error ocurred while fetching product for code:{code}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetByCodeAsync)}: Exit");
            return data;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetAllAsync)}: Initiating Get all products");
            IEnumerable<ProductDto> data = new List<ProductDto>();
            try
            {
                var entities = await _repository.GetAllAsync(p=>p.Dimensions);
                if (entities?.Any() ?? false)
                {
                    data = _mapper.Map<List<ProductDto>>(entities);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(GetAllAsync)}: An un-expected error ocurred while fetching products");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(GetAllAsync)}: Exit");
            return data;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Initiating drop product with id:{id}");
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
                    _logger.LogError($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Validation error ocurred while droping product with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(DeleteAsync)}: An un-expected error ocurred while deleting product for id:{id}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult> DeleteAsync(string code)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Initiating drop product with code:{code}");
            OperationResult opResult = new OperationResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    await _repository.DeleteAsync(code);
                    opResult.Success = true;
                    opResult.Message = "Record successfully deleted";
                }
                else
                {
                    _logger.LogError($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Validation error ocurred while droping product with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(DeleteAsync)}: An un-expected error ocurred while deleting product for Code:{code}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(DeleteAsync)}: Exit");
            return opResult;
        }
        public async Task<OperationResult> UploadProductsAsync(IEnumerable<ProductRequest> products)
        {
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Initiating upload products with Request:{JsonSerializer.Serialize(products)}");
            OperationResult opResult = new OperationResult { Success = false };
            try
            {
                if (products?.Any() ?? false)
                {
                    foreach (var product in products)
                    {
                        var validationResult = await _validator.ValidateAsync(product);
                        if (!validationResult.IsValid)
                        {
                            var error = string.Join(",", validationResult.Errors);
                            opResult.Message = error;
                            _logger.LogError($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Validation error ocurred while uploading product with Error:{error}");
                        }
                        else
                        {
                            var existingProduct = await _repository.GetFirstAsync(c => c.Code.ToLower() == product.Code.ToLower(), p=>p.Dimensions);
                            if (existingProduct != null)
                            {
                                _logger.LogInformation($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Product already exist in system with Code:{product.Code}. So updating");
                                if (product.Dimensions != null)
                                {
                                    if (existingProduct.Dimensions != null)
                                    {
                                        existingProduct.Dimensions.Length = product.Dimensions.Length;
                                        existingProduct.Dimensions.Width = product.Dimensions.Width;
                                        existingProduct.Dimensions.Weight = product.Dimensions.Weight;
                                        existingProduct.Dimensions.Height = product.Dimensions.Height;
                                    }
                                    else
                                    {
                                        existingProduct.Dimensions = _mapper.Map<Dimensions>(product.Dimensions);
                                    }
                                }

                                existingProduct.Title = product.Title!;
                                existingProduct.Description = product.Description!;
                                existingProduct.UpdatedDate = DateTime.UtcNow;
                                existingProduct.UpdatedBy = "[System]";
                                await _repository.UpdateAsync(existingProduct);
                            }
                            else
                            {
                                _logger.LogInformation($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Product dpesn't exist in system with Code:{product.Code}. So Adding");
                                var entity = _mapper.Map<Product>(product);
                                await _repository.AddAsync(entity);
                            }
                        }
                    }
                    opResult.Success = true;
                    opResult.Message = "Products successfully uploaded";
                    _logger.LogInformation($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Successfully uploaded products with Request:{JsonSerializer.Serialize(products)}");
                }
                else
                {
                    _logger.LogError($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Validation error ocurred while uploading products with Error:No products to upload");
                    opResult.Message = "No products to upload";
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: An un-expected error ocurred while uploading products with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(ProductService)}.{nameof(UploadProductsAsync)}: Exit");
            return opResult;
        }
    }
}
