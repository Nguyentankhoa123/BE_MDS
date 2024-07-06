using System.ComponentModel.DataAnnotations;

namespace MDS.Services.DTO.Account
{
    public class AccountRegisterRequest
    {
        [Required]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
