using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GAC.WMS.Application.Tests;
using Microsoft.AspNetCore.Http;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Tests.Controller
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _serviceMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _serviceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenProductsExist()
        {
            var products = new List<ProductDto> { Setup.GetProductDto(1), Setup.GetProductDto(2)};
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenNoProductsExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ProductDto>());

            var result = await _controller.Get();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No products found", notFound.Value);
        }

        [Fact]
        public async Task Get_ShouldHandleException()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("error"));

            var result = await _controller.Get();

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenProductExists()
        {
            var product = Setup.GetProductDto(1);
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(product);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync((ProductDto)null!);

            var result = await _controller.Get(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No product found with id:1", notFound.Value);
        }

        [Fact]
        public async Task GetById_ShouldHandleException()
        {
            _serviceMock.Setup(s => s.GetAsync(1)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Get(1);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task GetByCode_ShouldReturnOk_WhenProductExists()
        {
            var product = Setup.GetProductDto(1);
            _serviceMock.Setup(s => s.GetByCodeAsync("P1")).ReturnsAsync(product);

            var result = await _controller.GetByCode("P1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetByCode_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetByCodeAsync("P1")).ReturnsAsync((ProductDto)null!);

            var result = await _controller.GetByCode("P1");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No product found with code:P1", notFound.Value);
        }

        [Fact]
        public async Task GetByCode_ShouldHandleException()
        {
            _serviceMock.Setup(s => s.GetByCodeAsync("P1")).ThrowsAsync(new Exception("error"));

            var result = await _controller.GetByCode("P1");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnCreated_WhenProductIsCreated()
        {
            var product = Setup.GetProductRequest(1);
            var opResult = new OperationResult<ProductDto> { Success = true, Data = Setup.GetProductDto(1) };
            _serviceMock.Setup(s => s.CreateAsync(product)).ReturnsAsync(opResult);

            var result = await _controller.Post(product);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(opResult.Data, created.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Post(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenCreationFails()
        {
            var product = Setup.GetProductRequest(1);
            var opResult = new OperationResult<ProductDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.CreateAsync(product)).ReturnsAsync(opResult);

            var result = await _controller.Post(product);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Post_ShouldHandleException()
        {
            var product = Setup.GetProductRequest(1);
            _serviceMock.Setup(s => s.CreateAsync(product)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Post(product);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenProductIsUpdated()
        {
            var product = Setup.GetProductRequest(1);
            var opResult = new OperationResult<ProductDto> { Success = true, Data = Setup.GetProductDto(1) };
            _serviceMock.Setup(s => s.UpdateAsync(1, product)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, product);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult.Data, ok.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Put(1, null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var product = Setup.GetProductRequest(1);
            var opResult = new OperationResult<ProductDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.UpdateAsync(1, product)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, product);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Put_ShouldHandleException()
        {
            var product = Setup.GetProductRequest(1);
            _serviceMock.Setup(s => s.UpdateAsync(1, product)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Put(1, product);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            var opResult = new OperationResult { Success = true };
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(opResult);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenDeletionFails()
        {
            var opResult = new OperationResult { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(opResult);

            var result = await _controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Delete_ShouldHandleException()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Delete(1);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            var opResult = new OperationResult { Success = true };
            _serviceMock.Setup(s => s.DeleteAsync("P1")).ReturnsAsync(opResult);

            var result = await _controller.DeleteByCode("P1");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnBadRequest_WhenDeletionFails()
        {
            var opResult = new OperationResult { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.DeleteAsync("P1")).ReturnsAsync(opResult);

            var result = await _controller.DeleteByCode("P1");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task DeleteByCode_ShouldHandleException()
        {
            _serviceMock.Setup(s => s.DeleteAsync("P1")).ThrowsAsync(new Exception("error"));

            var result = await _controller.DeleteByCode("P1");

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenProductsIsNull()
        {
            // Act
            var result = await _controller.Upload(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenProductsIsEmpty()
        {
            // Act
            var result = await _controller.Upload(new List<ProductRequest>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var products = new List<ProductRequest> { Setup.GetProductRequest(1) };
            var opResult = new OperationResult { Success = true, Message = "Uploaded" };
            _serviceMock.Setup(s => s.UploadProductsAsync(products)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(products);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult, okResult.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var products = new List<ProductRequest> { Setup.GetProductRequest(1) };
            var opResult = new OperationResult { Success = false, Message = "Failed" };
            _serviceMock.Setup(s => s.UploadProductsAsync(products)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(products);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var products = new List<ProductRequest> { Setup.GetProductRequest(1) };
            _serviceMock.Setup(s => s.UploadProductsAsync(products)).ThrowsAsync(new System.Exception("error"));

            // Act
            var result = await _controller.Upload(products);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
