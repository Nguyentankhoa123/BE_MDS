using MDS.Shared.Core.Helper;

namespace MDS.Model.Entity
{
    public class Discount
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
        public DiscountApply ApplyTo { get; set; } = DiscountApply.System;
        public string? DrugstoreId { get; set; }
        public ICollection<ApplicationUser> Drugstore { get; set; }
        public ICollection<DiscountUser> DiscountUsers { get; set; }

    }
}
