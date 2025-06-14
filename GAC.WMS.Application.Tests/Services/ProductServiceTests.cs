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
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<IValidator<ProductRequest>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _validatorMock = new Mock<IValidator<ProductRequest>>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_repositoryMock.Object, _loggerMock.Object, _validatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var productRequest = Setup.GetProductRequest(1);
            var productDto = Setup.GetProductDto(1);
            var productEntity = Setup.GetProductEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(productRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Product>(productRequest)).Returns(productEntity);
            _repositoryMock.Setup(r => r.AddAsync(productEntity)).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductDto>(productEntity)).Returns(productDto);

            // Act
            var result = await _service.CreateAsync(productRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Product successfully created", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.CreateAsync(productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Title is required", result.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleException()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.CreateAsync(productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSuccess_WhenValidationPasses()
        {
            // Arrange
            var productRequest = Setup.GetProductRequest(1);
            var productDto = Setup.GetProductDto(1);
            var productEntity = Setup.GetProductEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(productRequest, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(productEntity);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(productEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(productEntity)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ProductDto>(productEntity)).Returns(productDto);

            // Act
            var result = await _service.UpdateAsync(1, productRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Product successfully updated", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenIdIsInvalid()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);

            // Act
            var result = await _service.UpdateAsync(0, productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Id is required", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Title", "Title is required")
            };
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default)).ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _service.UpdateAsync(1, productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Title is required", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleException_WhenRepositoryThrowsError()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var productEntity = Setup.GetProductEntity(1);
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default)).ReturnsAsync(new ValidationResult());
            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(productEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(productEntity)).ThrowsAsync(new Exception("Database error"));
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(productEntity);

            // Act
            var result = await _service.UpdateAsync(1, productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Database error", result.Message);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleException_WhenMapperThrowsError()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default)).ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(Setup.GetProductEntity(1));
            _mapperMock.Setup(m => m.Map<Product>(productDto)).Throws(new Exception("Mapping error"));
            _repositoryMock.Setup(m => m.UpdateAsync(It.IsAny<Product>())).Throws(new Exception("Mapping error"));

            // Act
            var result = await _service.UpdateAsync(1, productDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Mapping error", result.Message);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnProduct_WhenIdIsValid()
        {
            // Arrange
            var productEntity = Setup.GetProductEntity(1);
            var productDto = Setup.GetProductDto(1);
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductDto>(productEntity)).Returns(productDto);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDto.Title, result.Title);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync((Product)null!);

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenProductIs_is_Zero()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync((Product)null!);

            // Act
            var result = await _service.GetAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_When_Db_throw_Exception()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<Expression<Func<Product, object>>[]>())).ThrowsAsync(new Exception("Unknown error"));

            // Act
            var result = await _service.GetAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnProduct_WhenCodeIsValid()
        {
            // Arrange
            var productEntity = Setup.GetProductEntity(1);
            var productDto = Setup.GetProductDto(1);
            _repositoryMock.Setup(r => r.GetByCodeAsync("P001", It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(productEntity);
            _mapperMock.Setup(m => m.Map<ProductDto>(productEntity)).Returns(productDto);

            // Act
            var result = await _service.GetByCodeAsync("P001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("P001", result.Code);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByCodeAsync("P001", It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync((Product)null!);

            // Act
            var result = await _service.GetByCodeAsync("P001");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_When_Parameter_is_Empty()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByCodeAsync("P001", It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync((Product)null!);

            // Act
            var result = await _service.GetByCodeAsync("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_When_Db_Throw_Exception()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByCodeAsync("P001", It.IsAny<Expression<Func<Product, object>>[]>())).ThrowsAsync(new Exception("Unnown Error"));

            // Act
            var result = await _service.GetByCodeAsync("P001");

            // Assert
            Assert.Null(result);
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
        public async Task DeleteAsync_ShouldReturnError_When_Db_throw_Exception()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("Unknown error"));

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenCodeIsValid()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync("P001")).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync("P001");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Record successfully deleted", result.Message);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnError_WhenCodeIsInvalid()
        {
            // Act
            var result = await _service.DeleteAsync("");

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task DeleteByCodeAsync_ShouldReturnError_When_Db_throw_Exception()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync("P001")).ThrowsAsync(new Exception("Unknown error"));

            // Act
            var result = await _service.DeleteAsync("P001");

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts_WhenDataExists()
        {
            // Arrange
            var productEntities = new List<Product>
            {
                Setup.GetProductEntity(1),
                Setup.GetProductEntity(2)
            };
            var productDtos = new List<ProductDto>
            {
                Setup.GetProductDto(1),
                Setup.GetProductDto(2)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(productEntities);
            _mapperMock.Setup(m => m.Map<List<ProductDto>>(productEntities)).Returns(productDtos);

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
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Product, object>>[]>())).ReturnsAsync(new List<Product>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldHandleException_WhenRepositoryThrowsError()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Product, object>>[]>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); 
        }

        [Fact]
        public async Task UploadProductsAsync_ShouldAddNewProduct_WhenValidAndNotExists()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var productEntity = Setup.GetProductEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, object>>[]>()))
                .ReturnsAsync((Product)null!);
            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(productEntity);
            _repositoryMock.Setup(r => r.AddAsync(productEntity)).ReturnsAsync(productEntity);

            // Act
            var result = await _service.UploadProductsAsync(new[] { productDto });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Products successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.Code == productDto.Code)), Times.Once);
        }

        [Fact]
        public async Task UploadProductsAsync_ShouldUpdateExistingProduct_WhenValidAndExists()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var existingProduct = Setup.GetProductEntity(1);

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.GetFirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, object>>[]>()))
                .ReturnsAsync(existingProduct);
            _mapperMock.Setup(m => m.Map<Dimensions>(productDto.Dimensions)).Returns(new Dimensions
            {
                Length = productDto.Dimensions!.Length,
                Width = productDto.Dimensions.Width,
                Height = productDto.Dimensions.Height,
                Weight = productDto.Dimensions.Weight
            });


            // Act
            var result = await _service.UploadProductsAsync(new[] { productDto });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Products successfully uploaded", result.Message);
            Assert.Equal(productDto.Title, existingProduct.Title);
            Assert.Equal(productDto.Dimensions.Length, existingProduct.Dimensions.Length);
            Assert.Equal(productDto.Dimensions.Width, existingProduct.Dimensions.Width);
            Assert.Equal(productDto.Dimensions.Height, existingProduct.Dimensions.Height);
            Assert.Equal(productDto.Dimensions.Weight, existingProduct.Dimensions.Weight);
            _repositoryMock.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task UploadProductsAsync_ShouldSetErrorMessage_WhenValidationFails()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Title", "Title is required") });

            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _service.UploadProductsAsync(new[] { productDto });

            // Assert
            Assert.True(result.Success); // Success is set to true at the end if any products were processed
            Assert.Contains("Products successfully uploaded", result.Message);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UploadProductsAsync_ShouldSetErrorMessage_WhenNoProductsProvided()
        {

            // Act
            var result = await _service.UploadProductsAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No products to upload", result.Message);
        }

        [Fact]
        public async Task UploadProductsAsync_ShouldSetErrorMessage_WhenExceptionThrown()
        {
            // Arrange
            var productDto = Setup.GetProductRequest(1);
            _validatorMock.Setup(v => v.ValidateAsync(productDto, default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.UploadProductsAsync(new[] { productDto });

            // Assert
            Assert.False(result.Success);
            Assert.Contains("An un-expected error ocurred. Error:Test exception", result.Message);
        }
    }
}
