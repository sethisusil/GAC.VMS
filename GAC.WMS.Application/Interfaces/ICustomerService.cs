using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<OperationResult<CustomerDto>> CreateAsync(CustomerRequest customer);
        Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerRequest customer);
        Task<CustomerDto> GetAsync(int id);
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult> UploadCustomersAsync(IEnumerable<CustomerRequest> customers);
    }
}
