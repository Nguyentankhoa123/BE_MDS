using MDS.Services.DTO.Brand;

namespace MDS.Services
{
    public interface IBrandService
    {
        Task<BrandObjectResponse> CreateAsync(BrandRequest request);
        Task<BrandObjectResponse> DeleteAsync(int id);
        Task<BrandListObjectResponse> GetAllAsync(int pageNumber, int pageSize);
        Task<BrandObjectResponse> GetByIdAsync(int id);
        Task<BrandWithProductsObjectResponse> GetProductsByBrandId(int id, int pageNumber, int pageSize);
    }
}
