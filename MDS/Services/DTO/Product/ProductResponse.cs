using MDS.Services.Common;
using MDS.Services.DTO.Comment;

namespace MDS.Services.DTO.Product
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string DrugstoreId { get; set; }
        public string DrugstoreName { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string DetailCategory { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public double Price { get; set; }
        public List<string> PictureUrls { get; set; }
        public string Description { get; set; }

        public bool Prescription { get; set; } // Thuốc có cần kê toa không

        public string ActiveIngredient { get; set; } // Hoạt chất

        public string DosageForm { get; set; } // Dạng bào chế

        public string? Use { get; set; } // Công dụng
        public int Quantity { get; set; }
        public string Note { get; set; }
        public int SoldQuantity { get; set; }
        public string ProductType { get; set; }
        public int Stock { get; set; }
        public DateTime? Created { get; set; }
        public ICollection<CommentResponse> Comments { get; set; }

    }

    public class ProductObjectResponse : ObjectResponse<ProductResponse> { }

    public class ProductListObjectResponse : ObjectResponse<List<ProductResponse>> { }
}
