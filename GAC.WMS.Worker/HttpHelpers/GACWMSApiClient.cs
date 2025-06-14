using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;
using GAC.WMS.Worker.Models;
using Microsoft.Extensions.Options;

namespace GAC.WMS.Worker.HttpHelpers
{
    public class GACWMSApiClient: BaseHttpClient, IGACWMSApiClient
    {
        private readonly ILogger<GACWMSApiClient> _logger;
        private readonly GacWmsApiConfig _gacWmsApiConfig;
        public GACWMSApiClient(HttpClient httpClient, 
            ILogger<GACWMSApiClient> logger,
            IOptions<GacWmsApiConfig> gacWmsApiConfigoptions) : base(httpClient, logger)
        {
            _logger= logger;
            _gacWmsApiConfig = gacWmsApiConfigoptions.Value ?? throw new ArgumentNullException(nameof(gacWmsApiConfigoptions), "GacWmsApiConfig cannot be null");
        }

        public async Task<OperationResult> UploadCustomer(IEnumerable<CustomerRequest> customers, CancellationToken cancellationToken)
        {
           _logger.LogInformation($"{nameof(GACWMSApiClient)}.{nameof(UploadCustomer)}: Uploading customers to GAC WMS API");
            return await PostAsync<OperationResult>(_gacWmsApiConfig.UploadCustomerUrl, customers, cancellationToken);
        }

        public async Task<OperationResult> UploadProducts(IEnumerable<ProductRequest> products, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GACWMSApiClient)}.{nameof(UploadProducts)}: Uploading products to GAC WMS API");
            return await PostAsync<OperationResult>(_gacWmsApiConfig.UploadProductUrl, products, cancellationToken);
        }

        public async Task<OperationResult> UploadPurchaseOrders(IEnumerable<PurchaseOrderRequest> purchaseOrders, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GACWMSApiClient)}.{nameof(UploadPurchaseOrders)}: Uploading purchase orders to GAC WMS API");
            return await PostAsync<OperationResult>(_gacWmsApiConfig.UploadPurchaseOrderUrl, purchaseOrders, cancellationToken);
        }

        public async Task<OperationResult> UploadSalesOrders(IEnumerable<SalesOrderRequest> salesOrders, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(GACWMSApiClient)}.{nameof(UploadSalesOrders)}: Uploading sales orders to GAC WMS API");
            return await PostAsync<OperationResult>(_gacWmsApiConfig.UploadSalesOrderUrl, salesOrders, cancellationToken);
        }
    }
}
