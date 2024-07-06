namespace MDS.Model.Entity
{
    public class DiscountUser
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public string UserId { get; set; }
        public DateTime UsedAt { get; set; }
        public Discount Discount { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsActive { get; set; }

    }
}
