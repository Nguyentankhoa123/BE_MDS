namespace MDS.Services.DTO.Order
{
    public class OrderRequest
    {
        public DateTime CreateOn { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingType { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Carrier { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
