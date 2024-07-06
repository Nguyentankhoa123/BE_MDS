using MDS.Services.DTO.Product;

namespace MDS.Services
{
    public interface IProductService
    {
        Task<MedicineObjectResponse> CreateMedicineAsync(string userId, MedicineRequest request);
        Task<NotMedicineObjectResponse> CreateNotMedicineAsync(string userId, NotMedicineRequest request);
        Task<ProductListObjectResponse> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductObjectResponse> GetByIdAsync(int id);
        Task<ProductObjectResponse> UpdateAsync(int id, ProductRequest request);
        Task<ProductObjectResponse> DeleteAsync(int id);
        Task<ProductListObjectResponse> GetDrugstoresForProduct(string id, int pageNumber, int pageSize);
        Task<ProductListObjectResponse> SearchAsync(string? nameQuery, string? activeIngredientQuery, string? useQuery, string? brandQuery, string? dosageFormQuery, string? filterQuery, string? priceSortOrder, int pageNumber, int pageSize);
    }
}
