using MDS.Services.Common;
using MDS.Services.DTO.Product;

namespace MDS.Services.DTO.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryWithProductsResponse : CategoryResponse
    {
        public List<ProductResponse> Products { get; set; }
    }

    public class CategoryObjectResponse : ObjectResponse<CategoryResponse> { }

    public class CategoryListObjectResponse : ObjectResponse<List<CategoryResponse>> { }

    public class CategoryWithProductsObjectResponse : ObjectResponse<CategoryWithProductsResponse> { }

}
