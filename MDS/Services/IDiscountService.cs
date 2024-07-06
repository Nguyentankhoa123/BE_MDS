using MDS.Services.DTO.Discount;

namespace MDS.Services
{
    public interface IDiscountService
    {
        Task<DiscountObjectResponse> CreateAsync(DiscountRequest request);
        Task<DiscountListObjectResponse> GetAllAsync(string? drugstoreId, int pageNumber, int pageSize);
        Task<DiscountObjectResponse> DeleteAsync(int discountId);
    }
}
