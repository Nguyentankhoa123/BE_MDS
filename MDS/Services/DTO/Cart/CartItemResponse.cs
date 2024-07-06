namespace MDS.Services.DTO.Cart
{
    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string DrugstoreId { get; set; }
        public string DrugstoreName { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public List<string> PictureUrls { get; set; }
        public int Quantity { get; set; }
    }
}
