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
    public class SalesOrderServiceTests
    {
        private readonly Mock<ISalesOrderRepository> _repositoryMock;
        private readonly Mock<IProductRepository> _productrepositoryMock;
        private readonly Mock<ICustomerRepository> _customerrepositoryMock;
        private readonly Mock<ILogger<SalesOrderService>> _loggerMock;
        private readonly Mock<IValidator<SalesOrderRequest>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SalesOrderService _service;

        public SalesOrderServiceTests()
        {
            _repositoryMock = new Mock<ISalesOrderRepository>();
            _productrepositoryMock = new Mock<IProductRepository>();
            _customerrepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<SalesOrderService>>();
            _validatorMock = new Mock<IValidator<SalesOrderRequest>>();
            _mapperMock = new Mock<IMapper>();
            _customerrepositoryMock.Setup(x=>x.GetWhereAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
                .ReturnsAsync(new List<Customer>());
            _service = new SalesOrderService(_repositoryMock.Object, _productrepositoryMock.Object, _customerrepositoryMock.Object, _loggerMock.Object, _validatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderDto(1);
            var salesOrderRequest = Setup.GetSalesOrderRequest(1);
            var salesOrderEntity =Setup.GetSalesOrderEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(salesOrderRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<SalesOrder>(salesOrderRequest)).Returns(salesOrderEntity);
            _repositoryMock.Setup(r => r.AddAsync(salesOrderEntity)).ReturnsAsync(salesOrderEntity);
            _mapperMock.Setup(m => m.Map<SalesOrderDto>(It.IsAny<SalesOrder>())).Returns(salesOrderDto);
            _customerrepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Setup.GetGetCustomerEntity(1));

            // Act
            var result = await _service.CreateAsync(salesOrderRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sales Order successfully created", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderRequest(1);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CustomerId", "CustomerId is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(salesOrderDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.CreateAsync(salesOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("CustomerId is required", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleException()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(salesOrderDto, default)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.CreateAsync(salesOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderDto(1);
            var salesOrderRequest = Setup.GetSalesOrderRequest(1);
            var salesOrderEntity = Setup.GetSalesOrderEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(salesOrderRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<SalesOrder>(salesOrderRequest)).Returns(salesOrderEntity);
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<SalesOrder, bool>>>(), It.IsAny<Expression<Func<SalesOrder, object>>[]>())).ReturnsAsync(salesOrderEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(salesOrderEntity)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<SalesOrderDto>(It.IsAny<SalesOrder>())).Returns(salesOrderDto);
            _customerrepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Setup.GetGetCustomerEntity(1));

            // Act
            var result = await _service.UpdateAsync(1, salesOrderRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("SalesOrder successfully updated", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderRequest(1);

            // Act
            var result = await _service.UpdateAsync(0, salesOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Id is required", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleException()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(salesOrderDto, default)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.UpdateAsync(1, salesOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnSalesOrder_WhenIdIsValid()
        {
            // Arrange
            var salesOrderDto = Setup.GetSalesOrderDto(1);
            var salesOrderEntity = Setup.GetSalesOrderEntity(1);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<SalesOrder, object>>[]>())).ReturnsAsync(salesOrderEntity);
            _mapperMock.Setup(m => m.Map<SalesOrderDto>(salesOrderEntity)).Returns(salesOrderDto);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenSalesOrderDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SalesOrder)null!);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSalesOrders_WhenDataExists()
        {
            // Arrange
            var salesOrderEntities = new List<SalesOrder>
            {
                Setup.GetSalesOrderEntity(1),
                Setup.GetSalesOrderEntity(2)
            };
            var salesOrderDtos = new List<SalesOrderDto>
            {
                Setup.GetSalesOrderDto(1),
                Setup.GetSalesOrderDto(2)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<SalesOrder, object>>[]>())).ReturnsAsync(salesOrderEntities);
            _mapperMock.Setup(m => m.Map<List<SalesOrderDto>>(salesOrderEntities)).Returns(salesOrderDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SalesOrder>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenIdIsValid()
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
        public async Task DeleteAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteAsync_ShouldHandleException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task UploadSalesOrdersAsync_ShouldAddNewSalesOrder_WhenValidAndNotExists()
        {
            // Arrange
            var soDto = Setup.GetSalesOrderRequest(1);
            var soEntity = Setup.GetSalesOrderEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(soDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<SalesOrder, bool>>>(), It.IsAny<Expression<Func<SalesOrder, object>>[]>()))
                .ReturnsAsync((SalesOrder)null!);
            _mapperMock.Setup(m => m.Map<SalesOrder>(soDto)).Returns(soEntity);
            _repositoryMock.Setup(r => r.AddAsync(soEntity)).ReturnsAsync(soEntity);

            // Act
            var result = await _service.UploadSalesOrdersAsync(new[] { soDto });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sales orders successfully uploaded", result.Message);
        }

        [Fact]
        public async Task UploadSalesOrdersAsync_ShouldUpdateExistingSalesOrder_WhenValidAndExists()
        {
            // Arrange
            var soDto = Setup.GetSalesOrderRequest(1);
            var existingSo = Setup.GetSalesOrderEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(soDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<SalesOrder, bool>>>(), It.IsAny<Expression<Func<SalesOrder, object>>[]>()))
                .ReturnsAsync(existingSo);

            // Act
            var result = await _service.UploadSalesOrdersAsync(new[] { soDto });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sales orders successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.UpdateAsync(existingSo), Times.Once);
        }

        [Fact]
        public async Task UploadSalesOrdersAsync_ShouldSetErrorMessage_WhenValidationFails()
        {
            // Arrange
            var soDto = Setup.GetSalesOrderRequest(1);
            var validationResult = new ValidationResult(new[] { new ValidationFailure("CustomerId", "CustomerId is required") });

            _validatorMock.Setup(v => v.ValidateAsync(soDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _service.UploadSalesOrdersAsync(new[] { soDto });

            // Assert
            Assert.True(result.Success); // Success is set to true at the end if any sales orders were processed
            Assert.Contains("Sales orders successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SalesOrder>()), Times.Never);
        }

        [Fact]
        public async Task UploadSalesOrdersAsync_ShouldSetErrorMessage_WhenNoSalesOrdersProvided()
        {
            // Act
            var result = await _service.UploadSalesOrdersAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No Sales Orders provided", result.Message);
        }

        [Fact]
        public async Task UploadSalesOrdersAsync_ShouldSetErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var soDto = Setup.GetSalesOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(soDto, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.UploadSalesOrdersAsync(new[] { soDto });

            // Assert
            Assert.False(result.Success);
            Assert.Contains("An un-expected error ocurred", result.Message);
        }
    }
}
