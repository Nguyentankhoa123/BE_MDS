namespace MDS.Services.DTO.FeedBack
{
    public class FeedBackRequest
    {
        public string UserId { get; set; }
        public string DrugstoreId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string RatingDescription { get; set; }
        public string Review { get; set; }
    }
}
