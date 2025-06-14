using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<OperationResult<PurchaseOrderDto>> CreateAsync(PurchaseOrderRequest purchaseOrder);
        Task<OperationResult<PurchaseOrderDto>> UpdateAsync(int id, PurchaseOrderRequest purchaseOrder);
        Task<PurchaseOrderDto> GetAsync(int id);
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync();
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult> UploadPurchaseOrdersAsync(IEnumerable<PurchaseOrderRequest> purchaseOrders);
    }
}
