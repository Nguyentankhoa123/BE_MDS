namespace MDS.Services.DTO.Account
{
    public class AddressRequest
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string UserId { get; set; }
        public bool IsDefault { get; set; }
    }
}