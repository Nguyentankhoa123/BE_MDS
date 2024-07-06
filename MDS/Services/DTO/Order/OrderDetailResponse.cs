using MDS.Services.Common;

namespace MDS.Services.DTO.Order
{
    public class OrderDetailResponse
    {
        public string DrugstoreId { get; set; }
        public string DrugstoreName { get; set; }
        public string OrderId { get; set; }
        public int ProductId { get; set; }
        public double Total { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrls { get; set; }
        public string Name { get; set; }
        public string OrderStatus { get; set; }
        public Boolean isReviewed { get; set; }
        public DateTime CreateOn { get; set; }
    }

    public class OrderDetailListObjectResponse : ObjectResponse<List<OrderDetailResponse>> { }
}
