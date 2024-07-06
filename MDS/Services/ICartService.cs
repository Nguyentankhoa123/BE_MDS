using MDS.Services.DTO.Cart;

namespace MDS.Services
{
    public interface ICartService
    {
        Task<CartObjectResponse> CreateAsync(string userId, int productId, int quantity);
        Task<CartObjectResponse> GetAsync(string userId);
        Task<CartObjectResponse> DecreaseAsync(string userId, int productId, int quantity);
        Task<CartObjectResponse> RemoveAsync(string userId, int productId);
        Task<CartObjectResponse> RemoveAllAsync(string userId);
    }
}
