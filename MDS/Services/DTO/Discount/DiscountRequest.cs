namespace MDS.Services.DTO.Discount
{
    public class DiscountRequest
    {
        public string Name { get; set; }
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUse { get; set; }
        public string? DrugstoreId { get; set; }
    }
}
