namespace MDS.Model.Entity
{
    public class Reservation
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
