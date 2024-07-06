namespace MDS.Model.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
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
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
