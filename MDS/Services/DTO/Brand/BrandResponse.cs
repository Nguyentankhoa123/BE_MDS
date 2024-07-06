using MDS.Services.Common;
using MDS.Services.DTO.Product;

namespace MDS.Services.DTO.Brand
{
    public class BrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BrandWithProductsResponse : BrandResponse
    {
        public List<ProductResponse> Products { get; set; }
    }

    public class BrandObjectResponse : ObjectResponse<BrandResponse> { }

    public class BrandListObjectResponse : ObjectResponse<List<BrandResponse>> { }

    public class BrandWithProductsObjectResponse : ObjectResponse<BrandWithProductsResponse> { }
}
