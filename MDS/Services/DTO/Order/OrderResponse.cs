using MDS.Services.Common;

namespace MDS.Services.DTO.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public DateTime CreateOn { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingType { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Carrier { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public double TotalPrice { get; set; }
        public double? DiscountAmount { get; set; }
        public string? PaymentUrl { get; set; }
        public ICollection<OrderDetailResponse> OrderDetails { get; set; }
    }

    public class OrderObjectResponse : ObjectResponse<OrderResponse> { }

    public class OrderListObjectResponse : ObjectResponse<List<OrderResponse>> { }
}
