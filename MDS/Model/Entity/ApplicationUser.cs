using Microsoft.AspNetCore.Identity;

namespace MDS.Model.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<Address> Address { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Discount> Discounts { get; set; }
    }
}
