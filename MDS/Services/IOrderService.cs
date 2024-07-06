using MDS.Services.DTO.Order;

namespace MDS.Services
{
    public interface IOrderService
    {
        Task<OrderObjectResponse> CreateAsync(OrderRequest request, string userId, string? code, int addressId, HttpContext context);
        Task<OrderListObjectResponse> GetAsync(string userId);
        Task<JmetterObjectResponse> TestOrderAsync(int productId, int quantity);
        Task<JmetterObjectResponse> TestOrderNoReisAsync(int productId, int quantity);
        Task<OrderDetailListObjectResponse> GetOrderDetailsByDrugstoreAsync(string drugstoreId, int pageNumber = 1, int pageSize = 5);

        Task<OrderListObjectResponse> TestOrderDrugstore(OrderRequest request, string userId, string? code, int addressId, HttpContext context);
    }
}
