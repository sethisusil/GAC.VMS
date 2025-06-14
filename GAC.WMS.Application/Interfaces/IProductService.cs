using GAC.WMS.Core.Dtos;
using GAC.WMS.Core.Request;

namespace GAC.WMS.Application.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult<ProductDto>> CreateAsync(ProductRequest product);
        Task<OperationResult<ProductDto>> UpdateAsync(int id, ProductRequest product);
        Task<ProductDto> GetAsync(int id);
        Task<ProductDto> GetByCodeAsync(string code);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult> DeleteAsync(string code);
        Task<OperationResult> UploadProductsAsync(IEnumerable<ProductRequest> products);
    }
}
