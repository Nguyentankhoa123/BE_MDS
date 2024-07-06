using MDS.Services.Common;

namespace MDS.Services.DTO.FeedBack
{
    public class FeedBackResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DrugstoreId { get; set; }
        public int ProductId { get; set; }
        public string DrugstoreName { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string RatingDescription { get; set; }
        public string Review { get; set; }
    }

    public class AllDrugstoreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public int Rating { get; set; }
    }

    public class FeedBackObjectResponse : ObjectResponse<FeedBackResponse> { }
    public class FeedBackListObjectResponse : ObjectResponse<List<FeedBackResponse>> { }


    public class AllDrugstoreListObjectResponse : ObjectResponse<List<AllDrugstoreResponse>> { }
}
