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
    public class CustomerService : ICustomerService
    {
        public readonly ICustomerRepository _repository;
        private readonly ILogger<CustomerService> _logger;
        private readonly IValidator<CustomerRequest> _validator;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository repository,
            ILogger<CustomerService> logger,
            IValidator<CustomerRequest> validator,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<OperationResult<CustomerDto>> CreateAsync(CustomerRequest customer)
        {
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(CreateAsync)}: Initiating create customer with Request:{JsonSerializer.Serialize(customer)}");
            OperationResult<CustomerDto> opResult = new OperationResult<CustomerDto> { Success = false };
            try
            {
                var validationResult = await _validator.ValidateAsync(customer);
                if (!validationResult.IsValid)
                {
                    var error = string.Join(",", validationResult.Errors);
                    opResult.Message = error;
                    _logger.LogError($"{nameof(CustomerService)}.{nameof(CreateAsync)}: Validation error ocurred while creating customer with Error:{error}");
                }
                else
                {
                    var existingCustomer = await _repository.GetFirstAsync(c => c.Email.ToLower() == customer.Email!.ToLower());
                    if (existingCustomer == null)
                    {
                        var entity = _mapper.Map<Customer>(customer);
                        await _repository.AddAsync(entity);
                        opResult.Success = true;
                        opResult.Message = "Customer successfully created";
                        opResult.Data = _mapper.Map<CustomerDto>(entity);
                        _logger.LogInformation($"{nameof(CustomerService)}.{nameof(CreateAsync)}: Successfully created customer with customerId:{entity.Id} for Request:{JsonSerializer.Serialize(customer)}");
                    }
                    else
                    {
                        _logger.LogError($"{nameof(CustomerService)}.{nameof(CreateAsync)}: Customer already exist in system with Email:{customer.Email}. So not creating again");
                        opResult.Message = $"Customer already exist in system with Email:{customer.Email}. So not creating again";
                        opResult.Success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(CreateAsync)}: An un-expected error ocurred while creating customer with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(CreateAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerRequest customer)
        {
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Initiating update customer for id:{id} with Request:{JsonSerializer.Serialize(customer)}");
            OperationResult<CustomerDto> opResult = new OperationResult<CustomerDto> { Success = false };
            try
            {
                if (id > 0)
                {
                    var validationResult = await _validator.ValidateAsync(customer);
                    if (!validationResult.IsValid)
                    {
                        var error = string.Join(",", validationResult.Errors);
                        opResult.Message = error;
                        _logger.LogError($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating customer for Id:{id} with Error:{error}");
                    }
                    else
                    {
                        var existingCustomer = await _repository.GetByIdAsync(id, x=>x.Address);
                        if(existingCustomer == null)
                        {
                            _logger.LogError($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Record does not exist with customerId:{id}");
                            opResult.Message = "Record does not exist";
                            return opResult;
                        }
                        var customerWithName = await _repository.GetFirstAsync(c => c.Email.ToLower() == customer.Email!.ToLower() && c.Id != id);
                        if (customerWithName != null)
                        {
                            _logger.LogError($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Customer already exist in system with Email:{customer.Email}. So not updating");
                            opResult.Message = $"Customer already exist in system with Email:{customer.Email}. So not updating";
                            return opResult;
                        }
                        existingCustomer.Email = customer.Email;
                        if (customer.Address != null)
                        {
                            if (existingCustomer.Address == null)
                            {
                                existingCustomer.Address = _mapper.Map<Address>(customer.Address);
                            }
                            else
                            {
                                existingCustomer.Address.Street = customer.Address.Street!;
                                existingCustomer.Address.City = customer.Address.City!;
                                existingCustomer.Address.State = customer.Address.State!;
                                existingCustomer.Address.ZipCode = customer.Address.ZipCode!;
                            }
                        }
                        await _repository.UpdateAsync(existingCustomer);
                        opResult.Success = true;
                        opResult.Message = "Customer successfully updated";
                        opResult.Data = _mapper.Map<CustomerDto>(existingCustomer);
                        _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Successfully updated customer with customerId:{existingCustomer.Id} for Request:{JsonSerializer.Serialize(customer)}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Validation error ocurred while updating customer with Error:Id is required");
                    opResult.Message = "Id is required";
                    opResult.Success = false;
                }

            }
            catch (Exception ex)
            {
                opResult.Success = false;
                opResult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(UpdateAsync)}: An un-expected error ocurred while updating for id:{id} customer with Error:{ex.Message}");
            }
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Exit");
            return opResult;
        }

        public async Task<CustomerDto> GetAsync(int id)
        {
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(GetAsync)}: Initiating Get customer with id:{id}");
            CustomerDto data = null!;
            try
            {
                if (id > 0)
                {
                    var entity = await _repository.GetByIdAsync(id, c => c.Address);
                    if (entity != null)
                    {
                        data = _mapper.Map<CustomerDto>(entity);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(CustomerService)}.{nameof(UpdateAsync)}: Record does not exist with customerId:{id}");
                    }
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerService)}.{nameof(GetAsync)}: Validation error ocurred while fetching customer with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(GetAsync)}: An un-expected error ocurred while fetching customer for id:{id}");
            }
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(GetAsync)}: Exit");
            return data;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(GetAllAsync)}: Initiating Get all customers");
            IEnumerable<CustomerDto> data = new List<CustomerDto>();
            try
            {
                var entities = await _repository.GetAllAsync(c => c.Address);
                if (entities?.Any() ?? false)
                {
                    data = _mapper.Map<List<CustomerDto>>(entities);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(GetAllAsync)}: An un-expected error ocurred while fetching customers");
            }
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(GetAllAsync)}: Exit");
            return data;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(DeleteAsync)}: Initiating drop customer with id:{id}");
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
                    _logger.LogError($"{nameof(CustomerService)}.{nameof(DeleteAsync)}: Validation error ocurred while droping customer with Error:Id is required");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(DeleteAsync)}: An un-expected error ocurred while deleting customer for id:{id}");
            }
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(DeleteAsync)}: Exit");
            return opResult;
        }

        public async Task<OperationResult> UploadCustomersAsync(IEnumerable<CustomerRequest> customers)
        {
            OperationResult opresult = new OperationResult();
            _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: Initiating upload customers with Request:{JsonSerializer.Serialize(customers)}");
            try
            {
                if (customers?.Any() ?? false)
                {
                    foreach (var customer in customers)
                    {
                        var validationResult = await _validator.ValidateAsync(customer);
                        if (!validationResult.IsValid)
                        {
                            var error = string.Join(",", validationResult.Errors);
                            opresult.Message = error;
                            _logger.LogError($"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: Validation error ocurred while uploading customer with Error:{error}");
                        }
                        else
                        {
                            var existingCustomer = await _repository.GetFirstAsync(c => c.Email.ToLower() == customer.Email!.ToLower(), x=>x.Address);
                            if (existingCustomer != null)
                            {
                                _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: Customer already exist in system with Email:{customer.Email}. So updating");
                                if (customer.Address != null)
                                {
                                    if (existingCustomer.Address == null)
                                    {
                                        existingCustomer.Address = _mapper.Map<Address>(customer.Address);
                                    }
                                    else
                                    {
                                        existingCustomer.Address.Street = customer.Address.Street!;
                                        existingCustomer.Address.City = customer.Address.City!;
                                        existingCustomer.Address.State = customer.Address.State!;
                                        existingCustomer.Address.ZipCode = customer.Address.ZipCode!;
                                    }
                                }
                                await _repository.UpdateAsync(existingCustomer);
                            }
                            else
                            {
                                _logger.LogInformation($"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: Customer dpesn't exist in system with Email:{customer.Email}. So Adding");
                                var entity = _mapper.Map<Customer>(customer);
                                await _repository.AddAsync(entity);
                            }
                        }
                    }
                    opresult.Success = true;
                    opresult.Message = "Customers successfully uploaded";
                }
                else
                {
                    _logger.LogError($"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: Validation error ocurred while uploading customers with Error:Customers are required");
                    opresult.Message = "Customers are required";
                }
            }
            catch (Exception ex)
            {
                opresult.Success = false;
                opresult.Message = $"An un-expected error ocurred. Error:{ex.Message}";
                _logger.LogError(ex, $"{nameof(CustomerService)}.{nameof(UploadCustomersAsync)}: An un-expected error ocurred while uploading customers with Error:{ex.Message}");
            }
            return opresult;
        }

    }
}
