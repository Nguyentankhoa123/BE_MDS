using MDS.Services.DTO.FeedBack;

namespace MDS.Services
{
    public interface IFeedBackService
    {
        Task<FeedBackObjectResponse> CreateAsync(FeedBackRequest request);
        Task<FeedBackListObjectResponse> GetFeedbacksByDrugstore(string drugstoreId);

        //Task<AllDrugstoreListObjectResponse> GetAllDrugstores();
    }
}
