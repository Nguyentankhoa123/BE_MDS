namespace MDS.Model.Entity
{
    public class Inventory
    {
        public int Id { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string DrugstoreId { get; set; }
        public ApplicationUser Drugstore { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
