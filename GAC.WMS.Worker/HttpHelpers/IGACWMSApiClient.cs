using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Worker.HttpHelpers
{
    public interface IGACWMSApiClient
    {
        Task<OperationResult> UploadCustomer(IEnumerable<CustomerRequest> customers, CancellationToken cancellationToken);
        Task<OperationResult> UploadProducts(IEnumerable<ProductRequest> products, CancellationToken cancellationToken);
        Task<OperationResult> UploadPurchaseOrders(IEnumerable<PurchaseOrderRequest> purchaseOrders, CancellationToken cancellationToken);
        Task<OperationResult> UploadSalesOrders(IEnumerable<SalesOrderRequest> salesOrders, CancellationToken cancellationToken);
    }
}

