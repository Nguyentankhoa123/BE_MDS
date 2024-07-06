using System.ComponentModel.DataAnnotations;

namespace MDS.Services.DTO.Account
{
    public class AccountLoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
