using MDS.Services.Common;

namespace MDS.Services.DTO.Account
{
    public class AddressResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AddressObjectResponse : ObjectResponse<AddressResponse> { }

    public class AddressListObjectResponse : ObjectResponse<List<AddressResponse>> { }
}
