using MDS.Services.Common;

namespace MDS.Services.DTO.Inventory
{
    public class InventoryResponse
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
    }

    public class InventoryObjectResponse : ObjectResponse<InventoryResponse> { }
}
