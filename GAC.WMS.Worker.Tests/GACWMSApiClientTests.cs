using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;
using GAC.WMS.Worker.HttpHelpers;
using GAC.WMS.Worker.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GAC.WMS.Worker.Tests
{
    public class GACWMSApiClientTests
    {
        private readonly GacWmsApiConfig _config = new()
        {
            BaseUrl = "http://localhost",
            UploadCustomerUrl = "/customers",
            UploadProductUrl = "/products",
            UploadPurchaseOrderUrl = "/purchaseorders",
            UploadSalesOrderUrl = "/salesorders"
        };

        private readonly Mock<ILogger<GACWMSApiClient>> _loggerMock = new();

        // Testable subclass to override PostAsync
        private class TestGACWMSApiClient : GACWMSApiClient
        {
            public Func<string, object, CancellationToken, Task<OperationResult>>? PostAsyncOverride { get; set; }

            public TestGACWMSApiClient(HttpClient httpClient, ILogger<GACWMSApiClient> logger, IOptions<GacWmsApiConfig> config)
                : base(httpClient, logger, config) { }

            protected override async Task<T> PostAsync<T>(string url, object data, CancellationToken cancellationToken)
            {
                if (typeof(T) == typeof(OperationResult) && PostAsyncOverride != null)
                {
                    var result = await PostAsyncOverride(url, data, cancellationToken);
                    return (T)(object)result!;
                }
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task UploadCustomer_CallsPostAsyncAndReturnsResult()
        {
            var expected = new OperationResult { Success = true, Message = "ok" };
            var client = new TestGACWMSApiClient(new HttpClient(), _loggerMock.Object, Options.Create(_config))
            {
                PostAsyncOverride = (url, data, token) => Task.FromResult(expected)
            };

            var result = await client.UploadCustomer(new List<CustomerRequest>(), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        [Fact]
        public async Task UploadProducts_CallsPostAsyncAndReturnsResult()
        {
            var expected = new OperationResult { Success = true, Message = "ok" };
            var client = new TestGACWMSApiClient(new HttpClient(), _loggerMock.Object, Options.Create(_config))
            {
                PostAsyncOverride = (url, data, token) => Task.FromResult(expected)
            };

            var result = await client.UploadProducts(new List<ProductRequest>(), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        [Fact]
        public async Task UploadPurchaseOrders_CallsPostAsyncAndReturnsResult()
        {
            var expected = new OperationResult { Success = true, Message = "ok" };
            var client = new TestGACWMSApiClient(new HttpClient(), _loggerMock.Object, Options.Create(_config))
            {
                PostAsyncOverride = (url, data, token) => Task.FromResult(expected)
            };

            var result = await client.UploadPurchaseOrders(new List<PurchaseOrderRequest>(), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        [Fact]
        public async Task UploadSalesOrders_CallsPostAsyncAndReturnsResult()
        {
            var expected = new OperationResult { Success = true, Message = "ok" };
            var client = new TestGACWMSApiClient(new HttpClient(), _loggerMock.Object, Options.Create(_config))
            {
                PostAsyncOverride = (url, data, token) => Task.FromResult(expected)
            };

            var result = await client.UploadSalesOrders(new List<SalesOrderRequest>(), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("ok", result.Message);
        }

        [Fact]
        public async Task UploadCustomer_WhenPostAsyncThrows_PropagatesException()
        {
            var client = new TestGACWMSApiClient(new HttpClient(), _loggerMock.Object, Options.Create(_config))
            {
                PostAsyncOverride = (url, data, token) => throw new InvalidOperationException("fail")
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                client.UploadCustomer(new List<CustomerRequest>(), CancellationToken.None));
        }
    }
}
