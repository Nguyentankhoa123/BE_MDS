using MDS.Services.Common;
using MDS.Shared.Core.Helper;

namespace MDS.Services.DTO.Discount
{
    public class DiscountResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public DiscountApply ApplyTo { get; set; }
        public string? DrugstoreId { get; set; }
        public Boolean IsActive { get; set; }
        public string UserName { get; set; }
    }

    public class DiscountObjectResponse : ObjectResponse<DiscountResponse> { }

    public class DiscountListObjectResponse : ObjectResponse<List<DiscountResponse>> { }
}
