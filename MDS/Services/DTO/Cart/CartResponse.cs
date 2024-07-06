using MDS.Services.Common;

namespace MDS.Services.DTO.Cart
{
    public class CartResponse
    {
        public string UserId { get; set; }
        public List<CartItemResponse> CartItems { get; set; }
    }

    public class CartObjectResponse : ObjectResponse<CartResponse> { }
}
