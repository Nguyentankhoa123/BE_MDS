using MDS.Services.DTO.Category;

namespace MDS.Services
{
    public interface ICategoryService
    {
        Task<CategoryObjectResponse> CreateAsync(CategoryRequest request);
        Task<CategoryListObjectResponse> GetAllAsync(int pageNumber, int pageSize);
        Task<CategoryObjectResponse> GetByIdAsync(int id);
        Task<CategoryObjectResponse> DeleteAsync(int id);
        Task<CategoryWithProductsObjectResponse> GetProductsByCategoryId(int id, int pageNumber, int pageSize);
    }
}
