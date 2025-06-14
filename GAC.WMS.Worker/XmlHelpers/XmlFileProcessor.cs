using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;
using GAC.WMS.Worker.Extensions;
using GAC.WMS.Worker.HttpHelpers;
using GAC.WMS.Worker.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace GAC.WMS.Worker.XmlHelpers
{
    public class XmlFileProcessor : IXmlFileProcessor
    {
        private readonly ILogger<XmlFileProcessor> _logger;
        private readonly IGACWMSApiClient _apiClient;
        private readonly XmlFileConfig _xmlFileConfig;
        private const int BatchSize = 10; // Adjust batch size as needed
        private string subFolder = string.Empty;
        public XmlFileProcessor(ILogger<XmlFileProcessor> logger, IGACWMSApiClient apiClient, IOptions<XmlFileConfig> xmlFileConfigOptions)
        {
            _logger = logger;
            _apiClient = apiClient;
            _xmlFileConfig = xmlFileConfigOptions.Value ?? throw new ArgumentNullException(nameof(xmlFileConfigOptions), "XmlFileConfig cannot be null");
        }

        public async Task ProcessAsync()
        {
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Starting XML file processing");
            subFolder = $"{DateTime.Now.ToString("ddMMyyyyHHmmss")}";
            // Validate the XML file configuration
            if (_xmlFileConfig == null)
            {
                _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: XML file configuration is not set.");
                throw new ArgumentNullException(nameof(_xmlFileConfig), "XML file configuration cannot be null.");
            }

            // Ensure the base path exists
            if (string.IsNullOrWhiteSpace(_xmlFileConfig.BasePath))
            {
                _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Base path is not configured.");
                throw new ArgumentException("Base path is not configured.", nameof(_xmlFileConfig.BasePath));
            }

            if (!Directory.Exists(_xmlFileConfig.BasePath))
            {
                _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Base path {_xmlFileConfig.BasePath} does not exist.");
                throw new DirectoryNotFoundException($"Base path {_xmlFileConfig.BasePath} does not exist.");
            }
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Base path {_xmlFileConfig.BasePath} is valid and exists.");

            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Let's process customer file");
            //Proccess Customer File
            await ProcessCustomerFile();
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Customer file processed successfully.");

            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Let's process product file");
            //Proccess Product File
            await ProcessProductFile();
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Product file processed successfully.");

            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Let's process purchase order file");
            //Proccess Purchase Order File
            await ProcessPurchaseOrderFile();
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Purchase order file processed successfully.");

            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Let's process sales order file");
            //Proccess Sales Order File
            await ProcessSalesOrderFile();
            _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessAsync)}: Sales order file processed successfully.");
        }

        private async Task ProcessCustomerFile()
        {
            try
            {
                var filePath = Path.Combine(_xmlFileConfig.BasePath, _xmlFileConfig.CustomerFileName);
                if(!File.Exists(filePath))
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: Customer XML file does not exist at path:{filePath}");
                    return;
                }
                _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: Processing customer XML file from path:{filePath}");
                var customerList = XmlParser.DeserializeFromFile<CustomerList>(filePath);

                if (customerList?.Customers?.Any() ?? false)
                {
                    _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: Uploading {customerList.Customers.Count()} customers to GAC WMS API");
                    List<CustomerRequest> allCustomers = customerList.Customers;
                    var batchTasks = allCustomers
                                    .Batch(BatchSize)
                                    .Select(batch => _apiClient.UploadCustomer(batch, CancellationToken.None));

                    var result = await Task.WhenAll(batchTasks);
                    if (result?.Any(x => x.Success) ?? false)
                    {
                        _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: Successfully uploaded customers to GAC WMS API");
                        MoveFile(filePath, _xmlFileConfig.ProcessedPath);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: Failed to upload customers to GAC WMS API");
                    }
                }
                else
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: No customers found in XML file from path:{filePath}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(XmlFileProcessor)}.{nameof(ProcessCustomerFile)}: An error occurred while processing customer file - {ex.Message}");               
            }
        }

        private async Task ProcessProductFile()
        {
            try
            {
                var filePath = Path.Combine(_xmlFileConfig.BasePath, _xmlFileConfig.ProductFileName);
                if(!File.Exists(filePath))
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: Product XML file does not exist at path:{filePath}");
                    return;
                }
                _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: Processing product XML file from path:{filePath}");
                var productList = XmlParser.DeserializeFromFile<ProductList>(filePath);

                if (productList?.Products?.Any() ?? false)
                {
                    _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: Uploading {productList.Products.Count()} products to GAC WMS API");
                    List<ProductRequest> allProducts = productList.Products;
                    var batchTasks = allProducts
                                    .Batch(BatchSize)
                                    .Select(batch => _apiClient.UploadProducts(batch, CancellationToken.None));

                    var result = await Task.WhenAll(batchTasks);
                    if (result?.Any(x => x.Success) ?? false)
                    {
                        _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: Successfully uploaded products to GAC WMS API");
                        MoveFile(filePath, _xmlFileConfig.ProcessedPath);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: Failed to upload products to GAC WMS API");
                    }
                }
                else
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: No products found in XML file from path:{filePath}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(XmlFileProcessor)}.{nameof(ProcessProductFile)}: An error occurred while processing product file - {ex.Message}");
            }
        }

        private async Task ProcessPurchaseOrderFile()
        {
            try
            {
                var filePath = Path.Combine(_xmlFileConfig.BasePath, _xmlFileConfig.PurchaseOrderFileName);
               if(File.Exists(filePath) == false)
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: Purchase order XML file does not exist at path:{filePath}");
                    return;
                }
                _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: Processing purchase order XML file from path:{filePath}");
                var purchaseOrderList = XmlParser.DeserializeFromFile<PurchaseOrderList>(filePath);

                if (purchaseOrderList?.PurchaseOrders?.Any() ?? false)
                {
                    _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: Uploading {purchaseOrderList.PurchaseOrders.Count()} purchase orders to GAC WMS API");
                    List<PurchaseOrderRequest> allOrders = purchaseOrderList.PurchaseOrders.Select(x => new PurchaseOrderRequest
                    {
                        ProcessingDate= !string.IsNullOrWhiteSpace(x.ProcessingDate) ? DateTime.ParseExact(x.ProcessingDate,"dd/MM/yyyy",CultureInfo.InvariantCulture): null,
                        Customer =  x.Customer!=null ? new CustomerRequest
                        {
                            Name = x.Customer.Name,
                            Email = x.Customer.Email,
                            Address = x.Customer.Address!=null ? new AddressRequest
                            {
                                Street = x.Customer.Address?.Street,
                                City = x.Customer.Address?.City,
                                State = x.Customer.Address?.State,
                                ZipCode = x.Customer.Address?.ZipCode,
                                Country = x.Customer.Address?.Country
                            }: null
                        }: null,
                        Products = x.Products?.Select(p => new OrderItemDto
                        {
                            ProductCode = p.ProductCode,
                            Quantity = p.Quantity
                        }).ToList()
                    }).ToList();
                    var batchTasks = allOrders
                                    .Batch(BatchSize)
                                    .Select(batch => _apiClient.UploadPurchaseOrders(batch, CancellationToken.None));

                    var result = await Task.WhenAll(batchTasks);
                    if (result?.Any(x => x.Success) ?? false)
                    {
                        _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: Successfully uploaded purchase orders to GAC WMS API");
                        MoveFile(filePath, _xmlFileConfig.ProcessedPath);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: Failed to upload purchase orders to GAC WMS API");
                    }
                }
                else
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: No purchase orders found in XML file from path:{filePath}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(XmlFileProcessor)}.{nameof(ProcessPurchaseOrderFile)}: An error occurred while processing purchase order file - {ex.Message}");
            }
        }

        private async Task ProcessSalesOrderFile()
        {
            try
            {
                var filePath = Path.Combine(_xmlFileConfig.BasePath, _xmlFileConfig.SalesOrderFileName);
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: Sales order XML file does not exist at path:{filePath}");
                    return;
                }
                _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: Processing sales order XML file from path:{filePath}");
                var salesOrderList = XmlParser.DeserializeFromFile<SalesOrderList>(filePath);

                if (salesOrderList?.SalesOrders?.Any() ?? false)
                {
                    _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: Uploading {salesOrderList.SalesOrders.Count()} sales orders to GAC WMS API");
                    List<SalesOrderRequest> allOrders = salesOrderList.SalesOrders.Select(x => new SalesOrderRequest
                    {
                        ProcessingDate = !string.IsNullOrWhiteSpace(x.ProcessingDate) ? DateTime.ParseExact(x.ProcessingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.UtcNow,
                        Customer = x.Customer != null ? new CustomerRequest
                        {
                            Name = x.Customer.Name,
                            Email= x.Customer.Email,
                            Address = x.Customer.Address != null ? new AddressRequest
                            {
                                Street = x.Customer.Address?.Street,
                                City = x.Customer.Address?.City,
                                State = x.Customer.Address?.State,
                                ZipCode = x.Customer.Address?.ZipCode,
                                Country = x.Customer.Address?.Country
                            } : null
                        } : null,
                        Products = x.Products?.Select(p => new OrderItemDto
                        {
                            ProductCode = p.ProductCode,
                            Quantity = p.Quantity
                        }).ToList(),
                        ShipmentAddress = x.ShipmentAddress != null ? new AddressRequest
                        {
                            Street = x.ShipmentAddress.Street,
                            City = x.ShipmentAddress.City,
                            State = x.ShipmentAddress.State,
                            ZipCode = x.ShipmentAddress.ZipCode,
                            Country = x.ShipmentAddress.Country
                        } : null
                    }).ToList();
                    var batchTasks = allOrders
                                    .Batch(BatchSize)
                                    .Select(batch => _apiClient.UploadSalesOrders(batch, CancellationToken.None));

                    var result = await Task.WhenAll(batchTasks);
                    if (result?.Any(x => x.Success) ?? false)
                    {
                        _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: Successfully uploaded sales orders to GAC WMS API");
                        MoveFile(filePath, _xmlFileConfig.ProcessedPath);
                    }
                    else
                    {
                        _logger.LogError($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: Failed to upload sales orders to GAC WMS API");
                    }
                }
                else
                {
                    _logger.LogWarning($"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: No sales orders found in XML file from path:{filePath}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(XmlFileProcessor)}.{nameof(ProcessSalesOrderFile)}: An error occurred while processing sales order file - {ex.Message}");
            }
        }

        private void MoveFile(string filePath, string destinationFolder)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);                
                var destinationDirectory = Path.Combine(destinationFolder, subFolder);
                if(!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory); // Create subfolder if it doesn't exist
                }
                var destinationPath = Path.Combine(destinationDirectory, fileName);
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath); // Overwrite if exists
                }
                File.Move(filePath, destinationPath);
                _logger.LogInformation($"{nameof(XmlFileProcessor)}.{nameof(MoveFile)}: Successfully moved file {fileName} to {destinationFolder}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(XmlFileProcessor)}.{nameof(MoveFile)}: Failed to move file {filePath} to {destinationFolder}");
            }

        }
    }
}
