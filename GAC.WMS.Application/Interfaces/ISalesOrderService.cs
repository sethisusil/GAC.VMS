using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Interfaces
{
    public interface ISalesOrderService
    {
        Task<OperationResult<SalesOrderDto>> CreateAsync(SalesOrderRequest salesOrder);
        Task<OperationResult<SalesOrderDto>> UpdateAsync(int id, SalesOrderRequest salesOrder);
        Task<SalesOrderDto> GetAsync(int id);
        Task<IEnumerable<SalesOrderDto>> GetAllAsync();
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult> UploadSalesOrdersAsync(IEnumerable<SalesOrderRequest> salesOrders);
    }
}
