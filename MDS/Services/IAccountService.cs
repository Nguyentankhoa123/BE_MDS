using Google.Apis.Auth;
using MDS.Services.DTO.Account;
using MDS.Services.DTO.ExternalAuth;

namespace MDS.Services
{
    public interface IAccountService
    {
        Task<RegisterObjectResponse> RegisterAsync(AccountRegisterRequest request, string role);
        Task<TokenObjectResponse> LoginAsync(AccountLoginRequest request);
        Task<TokenObjectResponse> GetRefreshTokenAsync(TokenRequest request);
        Task<AddressObjectResponse> CreateAddressAsync(AddressRequest request);
        Task<AddressObjectResponse> UpdateAddressAsync(int addressId, AddressRequest request);
        Task<AddressListObjectResponse> GetByUserId(string userId);
        Task<AddressObjectResponse> DeleteAddressAsync(int id, string userId);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthRequest request);
        Task<TokenObjectResponse> GoogleLogin(ExternalAuthRequest request);
        Task<FacebookUserResponse> VerifyFacebookToken(ExternalAuthRequest request);
        Task<TokenObjectResponse> FacebookLogin(ExternalAuthRequest request);

    }
}
