namespace MDS.Model.Entity
{
    public class FeedBack
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DrugstoreId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string RatingDescription { get; set; }
        public string Review { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationUser Drugstore { get; set; }
    }
}
