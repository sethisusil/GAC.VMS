using GAC.WMS.Core.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using GAC.WMS.Application.Tests;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Api.Tests.Controller
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _serviceMock;
        private readonly Mock<ILogger<CustomerController>> _loggerMock;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _serviceMock = new Mock<ICustomerService>();
            _loggerMock = new Mock<ILogger<CustomerController>>();
            _controller = new CustomerController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenCustomersExist()
        {
            // Arrange
            var customers = new List<CustomerDto>
            {
                Setup.GetGetCustomerDto(1),
                Setup.GetGetCustomerDto(2)
            };
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customers, okResult.Value);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenNoCustomersExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CustomerDto>());

            // Act
            var result = await _controller.Get();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No customers found", notFoundResult.Value);
        }

        [Fact]
        public async Task Get_ShouldHandleException()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Get();

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCustomerExists()
        {
            // Arrange
            var customer = Setup.GetGetCustomerDto(1);
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync(customer);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customer, okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAsync(1)).ReturnsAsync((CustomerDto)null!);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No customers found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldHandleException()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetAsync(1)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Get(1);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnCreated_WhenCustomerIsCreated()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            var operationResult = new OperationResult<CustomerDto>
            {
                Success = true,
                Data = Setup.GetGetCustomerDto(1)
            };
            _serviceMock.Setup(s => s.CreateAsync(customer)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(operationResult.Data, createdResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Post(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenCreationFails()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            var operationResult = new OperationResult<CustomerDto>
            {
                Success = false,
                Message = "Validation error"
            };
            _serviceMock.Setup(s => s.CreateAsync(customer)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldHandleException()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            _serviceMock.Setup(s => s.CreateAsync(customer)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Post(customer);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenCustomerIsUpdated()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            var operationResult = new OperationResult<CustomerDto>
            {
                Success = true,
                Data = Setup.GetGetCustomerDto(1)
            };
            _serviceMock.Setup(s => s.UpdateAsync(1, customer)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Put(1, customer);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(operationResult.Data, okResult.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Put(1, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            var operationResult = new OperationResult<CustomerDto>
            {
                Success = false,
                Message = "Validation error"
            };
            _serviceMock.Setup(s => s.UpdateAsync(1, customer)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Put(1, customer);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequestResult.Value);
        }

        [Fact]
        public async Task Put_ShouldHandleException()
        {
            // Arrange
            var customer = Setup.GetGetCustomerRequest(1);
            _serviceMock.Setup(s => s.UpdateAsync(1, customer)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Put(1, customer);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCustomerIsDeleted()
        {
            // Arrange
            var operationResult = new OperationResult { Success = true };
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenDeletionFails()
        {
            // Arrange
            var operationResult = new OperationResult { Success = false, Message = "Validation error" };
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(operationResult);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation error", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ShouldHandleException()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenCustomersIsNull()
        {
            // Act
            var result = await _controller.Upload(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenCustomersIsEmpty()
        {
            // Act
            var result = await _controller.Upload(new List<CustomerRequest>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Request body cannot be null or empty", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var customers = new List<CustomerRequest> { Setup.GetGetCustomerRequest(1) };
            var opResult = new OperationResult { Success = true, Message = "Uploaded" };
            _serviceMock.Setup(s => s.UploadCustomersAsync(customers)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(customers);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(opResult, okResult.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            // Arrange
            var customers = new List<CustomerRequest> { Setup.GetGetCustomerRequest(1) };
            var opResult = new OperationResult { Success = false, Message = "Failed" };
            _serviceMock.Setup(s => s.UploadCustomersAsync(customers)).ReturnsAsync(opResult);

            // Act
            var result = await _controller.Upload(customers);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var customers = new List<CustomerRequest> { Setup.GetGetCustomerRequest(1) };
            _serviceMock.Setup(s => s.UploadCustomersAsync(customers)).ThrowsAsync(new System.Exception("error"));

            // Act
            var result = await _controller.Upload(customers);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
