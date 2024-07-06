using MDS.Services.Common;

namespace MDS.Services.DTO.Account
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class TokenObjectResponse : ObjectResponse<TokenResponse> { }
}
