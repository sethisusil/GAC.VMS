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
    public class PurchaseOrderControllerTests
    {
        private readonly Mock<IPurchaseOrderService> _serviceMock;
        private readonly Mock<ILogger<PurchaseOrderController>> _loggerMock;
        private readonly PurchaseOrderController _controller;

        public PurchaseOrderControllerTests()
        {
            _serviceMock = new Mock<IPurchaseOrderService>();
            _loggerMock = new Mock<ILogger<PurchaseOrderController>>();
            _controller = new PurchaseOrderController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenPurchaseOrdersExist()
        {
            var orders = new List<PurchaseOrderDto> { Setup.GetPurchaseOrderDto(1), Setup.GetPurchaseOrderDto(2) };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenNoPurchaseOrdersExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<PurchaseOrderDto>());

            var result = await _controller.Get();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No purchase orders found", notFound.Value);
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
        public async Task GetById_ShouldReturnOk_WhenPurchaseOrderExists()
        {
            var order = Setup.GetPurchaseOrderDto(1);
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(order);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenPurchaseOrderDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync((PurchaseOrderDto)null!);

            var result = await _controller.Get(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No purchase order found with id:1", notFound.Value);
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
        public async Task Post_ShouldReturnCreated_WhenPurchaseOrderIsCreated()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            var opResult = new OperationResult<PurchaseOrderDto> { Success = true, Data = Setup.GetPurchaseOrderDto(1) };
            _serviceMock.Setup(s => s.CreateAsync(order)).ReturnsAsync(opResult);

            var result = await _controller.Post(order);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(opResult.Data, created.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenCreationFails()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            var opResult = new OperationResult<PurchaseOrderDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.CreateAsync(order)).ReturnsAsync(opResult);

            var result = await _controller.Post(order);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Post_ShouldHandleException()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            _serviceMock.Setup(s => s.CreateAsync(order)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Post(order);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenPurchaseOrderIsUpdated()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            var opResult = new OperationResult<PurchaseOrderDto> { Success = true, Data = Setup.GetPurchaseOrderDto(1) };
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, order);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult.Data, ok.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            var opResult = new OperationResult<PurchaseOrderDto> { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ReturnsAsync(opResult);

            var result = await _controller.Put(1, order);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequest.Value);
        }

        [Fact]
        public async Task Put_ShouldHandleException()
        {
            var order = Setup.GetPurchaseOrderRequest(1);
            _serviceMock.Setup(s => s.UpdateAsync(1, order)).ThrowsAsync(new Exception("error"));

            var result = await _controller.Put(1, order);

            var status = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenPurchaseOrderIsDeleted()
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
        public async Task Upload_ReturnsBadRequest_WhenPurchaseOrdersIsNull()
        {
            // Act
            var result = await _controller.Upload(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenPurchaseOrdersIsEmpty()
        {
            // Act
            var result = await _controller.Upload(new List<PurchaseOrderRequest>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var purchaseOrders = new List<PurchaseOrderRequest> { Setup.GetPurchaseOrderRequest(1) };
            var opResult = new OperationResult { Success = true, Message = "Uploaded" };
            _serviceMock.Setup(s => s.UploadPurchaseOrdersAsync(purchaseOrders)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(purchaseOrders);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult, okResult.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var purchaseOrders = new List<PurchaseOrderRequest> { Setup.GetPurchaseOrderRequest(1) };
            var opResult = new OperationResult { Success = false, Message = "Failed" };
            _serviceMock.Setup(s => s.UploadPurchaseOrdersAsync(purchaseOrders)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(purchaseOrders);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var purchaseOrders = new List<PurchaseOrderRequest> { Setup.GetPurchaseOrderRequest(1) };
            _serviceMock.Setup(s => s.UploadPurchaseOrdersAsync(purchaseOrders)).ThrowsAsync(new System.Exception("error"));

            // Act
            var result = await _controller.Upload(purchaseOrders);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
