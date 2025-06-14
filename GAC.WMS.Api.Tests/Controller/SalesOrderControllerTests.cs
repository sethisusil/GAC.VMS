using GAC.WMS.Api.Controllers;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Application.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Tests.Controller
{
    public class SalesOrderControllerTests
    {
        private readonly Mock<ISalesOrderService> _serviceMock;
        private readonly Mock<ILogger<SalesOrderController>> _loggerMock;
        private readonly SalesOrderController _controller;

        public SalesOrderControllerTests()
        {
            _serviceMock = new Mock<ISalesOrderService>();
            _loggerMock = new Mock<ILogger<SalesOrderController>>();
            _controller = new SalesOrderController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenSalesOrdersExist()
        {
            var orders = new List<SalesOrderDto> { Setup.GetSalesOrderDto(1), Setup.GetSalesOrderDto(1) };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenNoSalesOrdersExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SalesOrderDto>());

            var result = await _controller.Get();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No sales orders found", notFound.Value);
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
        public async Task GetById_ShouldReturnOk_WhenSalesOrderExists()
        {
            var order = Setup.GetSalesOrderDto(1);
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(order);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenSalesOrderDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync((SalesOrderDto)null!);

            var result = await _controller.Get(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No sales order found with id:1", notFound.Value);
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
        public async Task Post_ShouldReturnCreated_WhenSalesOrderIsCreated()
        {
            var order = Setup.GetSalesOrderRequest(1);
            var opResult = new OperationResult<SalesOrderDto> { Success = true, Data = Setup.GetSalesOrderDto(1) };
            _serviceMock.Setup(s => s.CreateAsync(order)).ReturnsAsync(opResult);

            var result = await _controller.Post(order);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(opResult.Data, created.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Post(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid sales order data", badRequest.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenCreationFails()
        {
            var order = Setup.GetSalesOrderRequest(1);
            var opResult = new OperationResult<SalesOrderDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.CreateAsync(order)).ReturnsAsync(opResult);

            var result = await _controller.Post(order);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Post_ShouldHandleException()
        {
            var order = Setup.GetSalesOrderRequest(1);
            _serviceMock.Setup(s => s.CreateAsync(order)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Post(order);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenSalesOrderIsUpdated()
        {
            var order = Setup.GetSalesOrderRequest(1);
            var opResult = new OperationResult<SalesOrderDto> { Success = true, Data = Setup.GetSalesOrderDto(1) };
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, order);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult.Data, ok.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenModelIsNull()
        {
            var result = await _controller.Put(1, null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid sales order data", badRequest.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var order = Setup.GetSalesOrderRequest(1);
            var opResult = new OperationResult<SalesOrderDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, order);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Put_ShouldHandleException()
        {
            var order = Setup.GetSalesOrderRequest(1);
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Put(1, order);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSalesOrderIsDeleted()
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
        public async Task Upload_ReturnsBadRequest_WhenSalesOrdersIsNull()
        {
            // Act
            var result = await _controller.Upload(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No sales orders provided for upload", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenSalesOrdersIsEmpty()
        {
            // Act
            var result = await _controller.Upload(new List<SalesOrderRequest>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No sales orders provided for upload", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var salesOrders = new List<SalesOrderRequest> { Setup.GetSalesOrderRequest(1) };
            var opResult = new OperationResult { Success = true, Message = "Uploaded" };
            _serviceMock.Setup(s => s.UploadSalesOrdersAsync(salesOrders)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(salesOrders);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult, okResult.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var salesOrders = new List<SalesOrderRequest> { Setup.GetSalesOrderRequest(1) };
            var opResult = new OperationResult { Success = false, Message = "Failed" };
            _serviceMock.Setup(s => s.UploadSalesOrdersAsync(salesOrders)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(salesOrders);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var salesOrders = new List<SalesOrderRequest> { Setup.GetSalesOrderRequest(1) };
            _serviceMock.Setup(s => s.UploadSalesOrdersAsync(salesOrders)).ThrowsAsync(new Exception("error"));

            // Act
            var result = await _controller.Upload(salesOrders);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
