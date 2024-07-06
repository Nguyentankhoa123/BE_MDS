using MDS.Services.Common;

namespace MDS.Services.DTO.Account
{
    public class AccountRegisterResponse
    {

        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string Email { get; set; }

    }

    public class RegisterObjectResponse : ObjectResponse<AccountRegisterResponse> { }
}
