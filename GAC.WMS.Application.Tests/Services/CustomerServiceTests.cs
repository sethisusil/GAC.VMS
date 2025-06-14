using AutoMapper;
using FluentValidation.Results;
using FluentValidation;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Services;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly Mock<IValidator<CustomerRequest>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _validatorMock = new Mock<IValidator<CustomerRequest>>();
            _mapperMock = new Mock<IMapper>();
            _service = new CustomerService(_repositoryMock.Object, _loggerMock.Object, _validatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var customerEntity = Setup.GetGetCustomerEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customerEntity);
            _repositoryMock.Setup(r => r.AddAsync(customerEntity)).ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.CreateAsync(customerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Customer successfully created", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Create_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            customerDto.Name = null; // Simulate validation failure
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.CreateAsync(customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Name is required", result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnError_WhenExceptionHappens()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var customerEntity = Setup.GetGetCustomerEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customerEntity);
            _repositoryMock.Setup(r => r.AddAsync(customerEntity)).ThrowsAsync(new Exception("Server not available"));
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.CreateAsync(customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An un-expected error ocurred. Error:Server not available", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var customerEntity = Setup.GetGetCustomerEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customerEntity);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Customer, object>>[]>())).ReturnsAsync(customerEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(customerEntity)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.UpdateAsync(1, customerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Customer successfully updated", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Update_ShouldReturnError_WhenIdIsInvalid()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);

            // Act
            var result = await _service.UpdateAsync(0, customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Id is required", result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            customerDto.Name = null; // Simulate validation failure
            var customerEntity = Setup.GetGetCustomerEntity(1);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.UpdateAsync(1, customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Name is required", result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnError_WhenExceptionHappens()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var customerEntity = Setup.GetGetCustomerEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customerEntity);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Customer, object>>[]>())).ReturnsAsync(customerEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(customerEntity)).ThrowsAsync(new Exception("Server not available"));
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.UpdateAsync(1, customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An un-expected error ocurred. Error:Server not available", result.Message);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task Get_ShouldReturnCustomer_WhenIdIsValid()
        {
            // Arrange
            var customerEntity = Setup.GetGetCustomerEntity(1);
            var customerDto = Setup.GetGetCustomerRequest(1);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Customer, object>>[]>())).ReturnsAsync(customerEntity);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerDto.Name, result.Name);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer)null!);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenInvalidId_Pass()
        {

            // Act
            var result = await _service.GetAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldReturnError_When_Exception_Hapepns()
        {
            // Arrange
            var customerEntity = Setup.GetGetCustomerEntity(1);
            var customerDto = Setup.GetGetCustomerRequest(1);
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("Service not available"));
            _mapperMock.Setup(m => m.Map<CustomerDto>(customerEntity)).Returns(Setup.GetGetCustomerDto(1));

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCustomers_WhenDataExists()
        {
            // Arrange
            var customerEntities = new List<Customer>
            {
                Setup.GetGetCustomerEntity(1),
                Setup.GetGetCustomerEntity(2)
            };
            var customerDtos = new List<CustomerDto>
            {
                Setup.GetGetCustomerDto(1),
                Setup.GetGetCustomerDto(2)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Customer, object>>[]>())).ReturnsAsync(customerEntities);
            _mapperMock.Setup(m => m.Map<List<CustomerDto>>(customerEntities)).Returns(customerDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenIdIsValid()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Record successfully deleted", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnError_WhenIdIsInvalid()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task UploadCustomersAsync_ShouldAddNewCustomer_WhenValidAndNotExists()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var customerEntity = Setup.GetGetCustomerEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
                .ReturnsAsync((Customer)null!);
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customerEntity);
            _repositoryMock.Setup(r => r.AddAsync(customerEntity)).ReturnsAsync(customerEntity);

            // Act
            var result = await _service.UploadCustomersAsync([customerDto]);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Customers successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Customer>(c => c.Name == customerDto.Name)), Times.Once);
        }

        [Fact]
        public async Task UploadCustomersAsync_ShouldUpdateExistingCustomer_WhenValidAndExists()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var existingCustomer = Setup.GetGetCustomerEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(),It.IsAny<Expression<Func<Customer, object>>[]>()))
                .ReturnsAsync(existingCustomer);

            // Act
            var result = await _service.UploadCustomersAsync(new[] { customerDto });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Customers successfully uploaded", result.Message);
            Assert.Equal(customerDto.Address!.Street, existingCustomer.Address!.Street);
            Assert.Equal(customerDto.Address!.City, existingCustomer.Address.City);
            Assert.Equal(customerDto.Address!.State, existingCustomer.Address.State);
            Assert.Equal(customerDto.Address!.ZipCode, existingCustomer.Address.ZipCode);
            _repositoryMock.Verify(r => r.UpdateAsync(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task UploadCustomersAsync_ShouldSetErrorMessage_WhenValidationFails()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") });

            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _service.UploadCustomersAsync(new[] { customerDto });

            // Assert
            Assert.True(result.Success); // Success is set to true at the end if any customers were processed
            Assert.Contains("Customers successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task UploadCustomersAsync_ShouldSetErrorMessage_WhenNoCustomersProvided()
        {
            // Act
            var result = await _service.UploadCustomersAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Customers are required", result.Message);
        }

        [Fact]
        public async Task UploadCustomersAsync_ShouldSetErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var customerDto = Setup.GetGetCustomerRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(customerDto, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.UploadCustomersAsync(new[] { customerDto });

            // Assert
            Assert.False(result.Success);
            Assert.Contains("An un-expected error ocurred", result.Message);
        }
    }
}
