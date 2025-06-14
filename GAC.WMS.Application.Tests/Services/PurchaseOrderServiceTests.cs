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
    public class PurchaseOrderServiceTests
    {
        private readonly Mock<IPurchaseOrderRepository> _repositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<PurchaseOrderService>> _loggerMock;
        private readonly Mock<IValidator<PurchaseOrderRequest>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PurchaseOrderService _service;

        public PurchaseOrderServiceTests()
        {
            _repositoryMock = new Mock<IPurchaseOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<PurchaseOrderService>>();
            _validatorMock = new Mock<IValidator<PurchaseOrderRequest>>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerRepositoryMock.Setup(x=>x.GetWhereAsync(It.IsAny<Expression<Func<Customer, bool>>>()));
            _mapperMock = new Mock<IMapper>();
            _service = new PurchaseOrderService(_repositoryMock.Object, _productRepositoryMock.Object, _loggerMock.Object, _customerRepositoryMock.Object, _validatorMock.Object,  _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            var purchaseOrderEntity = Setup.GetPurchaseOrderEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<PurchaseOrder>(purchaseOrderRequest)).Returns(purchaseOrderEntity);
            _repositoryMock.Setup(r => r.AddAsync(purchaseOrderEntity)).ReturnsAsync(purchaseOrderEntity);
            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>())).Returns(purchaseOrderDto);
            _customerRepositoryMock.Setup(r=>r.GetFirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
                .ReturnsAsync(Setup.GetGetCustomerEntity(1));

            // Act
            var result = await _service.CreateAsync(purchaseOrderRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Purchase Order successfully created", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var purchaseOrderDto = new PurchaseOrderRequest { CustomerId = 0 };
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CustomerId", "CustomerId is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.CreateAsync(purchaseOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("CustomerId is required", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleException()
        {
            // Arrange
            var purchaseOrderDto = new PurchaseOrderRequest { CustomerId = 1 };
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderDto, default)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.CreateAsync(purchaseOrderDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            var purchaseOrderEntity = Setup.GetPurchaseOrderEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<PurchaseOrder>(purchaseOrderDto)).Returns(purchaseOrderEntity);
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<PurchaseOrder, bool>>>(),It.IsAny<Expression<Func<PurchaseOrder, object>>[]>())).ReturnsAsync(purchaseOrderEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(purchaseOrderEntity)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>())).Returns(purchaseOrderDto);
            _customerRepositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Expression<Func<Customer, object>>[]>()))
                .ReturnsAsync(Setup.GetGetCustomerEntity(1));

            // Act
            var result = await _service.UpdateAsync(1, purchaseOrderRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("PurchaseOrder successfully updated", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            // Act
            var result = await _service.UpdateAsync(0, purchaseOrderRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Id is required", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest= Setup.GetPurchaseOrderRequest(1);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CustomerId", "CustomerId is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.UpdateAsync(1, purchaseOrderRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("CustomerId is required", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleException()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.UpdateAsync(1, purchaseOrderRequest);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnPurchaseOrder_WhenIdIsValid()
        {
            // Arrange
            var purchaseOrderDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderEntity = Setup.GetPurchaseOrderEntity(1);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<PurchaseOrder, object>>[]>())).ReturnsAsync(purchaseOrderEntity);
            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(purchaseOrderEntity)).Returns(purchaseOrderDto);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenPurchaseOrderDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PurchaseOrder)null!);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenParameter_is_Invalid()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((PurchaseOrder)null!);

            // Act
            var result = await _service.GetAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_HandleException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("Unknown error"));

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPurchaseOrders_WhenDataExists()
        {
            // Arrange
            var purchaseOrderEntities = new List<PurchaseOrder>
            {
               Setup.GetPurchaseOrderEntity(1),
               Setup.GetPurchaseOrderEntity(2)
            };
            var purchaseOrderDtos = new List<PurchaseOrderDto>
            {
                Setup.GetPurchaseOrderDto(1),
                Setup.GetPurchaseOrderDto(2)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny< Expression<Func<PurchaseOrder, object>>[]>())).ReturnsAsync(purchaseOrderEntities);
            _mapperMock.Setup(m => m.Map<List<PurchaseOrderDto>>(purchaseOrderEntities)).Returns(purchaseOrderDtos);


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
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<PurchaseOrder>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_HandleException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Unknown Error"));

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
        public async Task UploadPurchaseOrdersAsync_ShouldAddNewPurchaseOrder_WhenValidAndNotExists()
        {
            // Arrange
            var poDto = Setup.GetPurchaseOrderDto(1);
            var poEntity = Setup.GetPurchaseOrderEntity(1);
            var productEntity = Setup.GetProductEntity(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<PurchaseOrder, bool>>>(),It.IsAny<Expression<Func<PurchaseOrder, object>>[]>()))
                .ReturnsAsync((PurchaseOrder)null!);
            _mapperMock.Setup(m => m.Map<PurchaseOrder>(poDto)).Returns(poEntity);
            _mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>())).Returns(productEntity);
            _repositoryMock.Setup(r => r.AddAsync(poEntity)).ReturnsAsync(poEntity);

            // Act
            var result = await _service.UploadPurchaseOrdersAsync(new[] { purchaseOrderRequest });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Purchase orders successfully uploaded", result.Message);
        }

        [Fact]
        public async Task UploadPurchaseOrdersAsync_ShouldUpdateExistingPurchaseOrder_WhenValidAndExists()
        {
            // Arrange
            var poDto = Setup.GetPurchaseOrderDto(1);
            var existingPo = Setup.GetPurchaseOrderEntity(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<PurchaseOrder, bool>>>(), It.IsAny<Expression<Func<PurchaseOrder, object>>[]>()))
                .ReturnsAsync(existingPo);
            // Act
            var result = await _service.UploadPurchaseOrdersAsync(new[] { purchaseOrderRequest });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Purchase orders successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.UpdateAsync(existingPo), Times.Once);
        }

        [Fact]
        public async Task UploadPurchaseOrdersAsync_ShouldSetErrorMessage_WhenValidationFails()
        {
            // Arrange
            var poDto = Setup.GetPurchaseOrderDto(1);
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            var validationResult = new ValidationResult([new ValidationFailure("CustomerId", "CustomerId is required")]);

            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _service.UploadPurchaseOrdersAsync(new[] { purchaseOrderRequest });

            // Assert
            Assert.True(result.Success); // Success is set to true at the end if any purchase orders were processed
            Assert.Contains("Purchase orders successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PurchaseOrder>()), Times.Never);
        }

        [Fact]
        public async Task UploadPurchaseOrdersAsync_ShouldSetErrorMessage_WhenNoPurchaseOrdersProvided()
        {

            // Act
            var result = await _service.UploadPurchaseOrdersAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Request is empty", result.Message);
        }

        [Fact]
        public async Task UploadPurchaseOrdersAsync_ShouldSetErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var purchaseOrderRequest = Setup.GetPurchaseOrderRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(purchaseOrderRequest, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.UploadPurchaseOrdersAsync([purchaseOrderRequest]);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("An un-expected error ocurred", result.Message);
        }
    }
}
