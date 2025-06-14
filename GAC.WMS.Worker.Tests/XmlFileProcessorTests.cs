using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GAC.WMS.Core.Dtos;
using GAC.WMS.Worker.HttpHelpers;
using GAC.WMS.Worker.Models;
using GAC.WMS.Worker.XmlHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GAC.WMS.Worker.Tests
{
    public class XmlFileProcessorTests
    {
        private readonly Mock<ILogger<XmlFileProcessor>> _loggerMock = new();
        private readonly Mock<IGACWMSApiClient> _apiClientMock = new();

        private XmlFileConfig GetValidConfig(string basePath)
        {
            return new XmlFileConfig
            {
                BasePath = basePath,
                ProcessedPath = Path.Combine(basePath, "processed"),
                CustomerFileName = "customers.xml",
                ProductFileName = "products.xml",
                PurchaseOrderFileName = "purchaseorders.xml",
                SalesOrderFileName = "salesorders.xml"
            };
        }

        private XmlFileProcessor CreateProcessor(XmlFileConfig config)
        {
            return new XmlFileProcessor(
                _loggerMock.Object,
                _apiClientMock.Object,
                Options.Create(config)
            );
        }

        [Fact]
        public async Task ProcessAsync_ThrowsIfConfigIsNull()
        {
            var options = new Mock<IOptions<XmlFileConfig>>();
            options.Setup(o => o.Value).Returns((XmlFileConfig)null!);

            Assert.Throws<ArgumentNullException>(() =>
                new XmlFileProcessor(_loggerMock.Object, _apiClientMock.Object, options.Object));
        }

        [Fact]
        public async Task ProcessAsync_ThrowsIfBasePathIsNullOrEmpty()
        {
            var config = GetValidConfig("");
            var processor = CreateProcessor(config);

            await Assert.ThrowsAsync<ArgumentException>(() => processor.ProcessAsync());
        }

        [Fact]
        public async Task ProcessAsync_ThrowsIfBasePathDoesNotExist()
        {
            var config = GetValidConfig(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            var processor = CreateProcessor(config);

            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => processor.ProcessAsync());
        }

        [Fact]
        public async Task ProcessAsync_WarnsIfCustomerFileMissing()
        {
            using var dir = new TempDir();
            var config = GetValidConfig(dir.Path);
            var processor = CreateProcessor(config);

            await processor.ProcessAsync();

            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Customer XML file does not exist")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WarnsIfProductFileMissing()
        {
            using var dir = new TempDir();
            // Create customer file so it proceeds to product
            File.WriteAllText(Path.Combine(dir.Path, "customers.xml"), "<CustomerList><Customers></Customers></CustomerList>");
            var config = GetValidConfig(dir.Path);
            var processor = CreateProcessor(config);

            await processor.ProcessAsync();

            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Product XML file does not exist")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WarnsIfPurchaseOrderFileMissing()
        {
            using var dir = new TempDir();
            File.WriteAllText(Path.Combine(dir.Path, "customers.xml"), "<CustomerList><Customers></Customers></CustomerList>");
            File.WriteAllText(Path.Combine(dir.Path, "products.xml"), "<ProductList><Products></Products></ProductList>");
            var config = GetValidConfig(dir.Path);
            var processor = CreateProcessor(config);

            await processor.ProcessAsync();

            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Purchase order XML file does not exist")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WarnsIfSalesOrderFileMissing()
        {
            using var dir = new TempDir();
            File.WriteAllText(Path.Combine(dir.Path, "customers.xml"), "<CustomerList><Customers></Customers></CustomerList>");
            File.WriteAllText(Path.Combine(dir.Path, "products.xml"), "<ProductList><Products></Products></ProductList>");
            File.WriteAllText(Path.Combine(dir.Path, "purchaseorders.xml"), "<PurchaseOrderList><PurchaseOrders></PurchaseOrders></PurchaseOrderList>");
            var config = GetValidConfig(dir.Path);
            var processor = CreateProcessor(config);

            await processor.ProcessAsync();

            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sales order XML file does not exist")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        // Add more tests for:
        // - Empty XML files (no entities)
        // - Successful upload (mock API returns success)
        // - Failed upload (mock API returns failure)
        // - Exception in file processing (simulate XmlParser or API throws)

        // Helper for temp directory
        private class TempDir : IDisposable
        {
            public string Path { get; }
            public TempDir()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(Path);
            }
            public void Dispose()
            {
                try { Directory.Delete(Path, true); } catch { }
            }
        }
    }
}
