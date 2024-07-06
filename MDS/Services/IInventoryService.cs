using MDS.Services.DTO.Inventory;

namespace MDS.Services
{
    public interface IInventoryService
    {
        Task<InventoryObjectResponse> InsertInventory(int productId, int stock, string drugstoreId);
        Task<int> ReservationInventory(int productId, int quantity, int cartId);
    }
}
