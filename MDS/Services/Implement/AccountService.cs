using AutoMapper;
using Google.Apis.Auth;
using MDS.Model.Entity;
using MDS.Services.DTO.Account;
using MDS.Services.DTO.ExternalAuth;
using MDS.Shared.Core.Enums;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Database.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace MDS.Services.Implement
{
    public class AccountService : IAccountService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IMapper _mapper;
        private AppDbContext _context;
        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public async Task<TokenObjectResponse> GetRefreshTokenAsync(TokenRequest request)
        {
            TokenObjectResponse response = new();

            var principal = GetPrincipalFromExpiredToken(request.AccessToken);

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new NotFoundException("Invalid access token or refresh token!");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.GivenName, user.FirstName),
               new Claim(ClaimTypes.Surname, user.LastName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var newAccessToken = GenerateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return response;
        }

        public async Task<TokenObjectResponse> LoginAsync(AccountLoginRequest request)
        {
            var response = new TokenObjectResponse();

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                throw new NotFoundException("Invalid Username");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new NotFoundException("Invalid Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(authClaims),
                RefreshToken = GenerateRefreshToken(),
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<RegisterObjectResponse> RegisterAsync(AccountRegisterRequest request, string role)
        {
            RegisterObjectResponse response = new();

            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                throw new BadRequestException("User already exists!");
            }

            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);

                var userResponse = _mapper.Map<AccountRegisterResponse>(user);

                response.StatusCode = ResponseCode.CREATED;
                response.Message = "Account created!";
                response.Data = userResponse;
            }
            else
            {
                var errorDescription = result.Errors.FirstOrDefault()?.Description ?? "Unknown error";
                response.StatusCode = ResponseCode.BADREQUEST;
                response.Message = $"Account creation failed: {errorDescription}";
            }


            return response;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var secret = _configuration["JWT:Secret"] ?? "";
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var expirationTimeUtc = DateTime.UtcNow.AddHours(1);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = expirationTimeInLocalTimeZone,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
        {
            var secret = _configuration["JWT:Secret"] ?? "";
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AddressObjectResponse> CreateAddressAsync(AddressRequest request)
        {
            AddressObjectResponse response = new();
            var address = _mapper.Map<Address>(request);

            // Nếu địa chỉ mới được đặt là mặc định, hủy bỏ địa chỉ mặc định cũ
            if (address.IsDefault)
            {
                var currentDefaultAddress = await _context.Addresss
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.IsDefault);
                if (currentDefaultAddress != null)
                {
                    currentDefaultAddress.IsDefault = false;
                    _context.Addresss.Update(currentDefaultAddress);
                }
            }

            _context.Addresss.Add(address);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.CREATED;
            response.Message = "Address created!";
            response.Data = _mapper.Map<AddressResponse>(address);

            return response;
        }

        public async Task<AddressObjectResponse> UpdateAddressAsync(int addressId, AddressRequest request)
        {
            AddressObjectResponse response = new();

            var existingAddress = await _context.Addresss
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Id == addressId);

            if (existingAddress == null)
            {
                throw new NotFoundException("Address not exists!");
            }

            // Nếu địa chỉ cập nhật được đặt là mặc định, hủy bỏ địa chỉ mặc định cũ
            if (request.IsDefault && !existingAddress.IsDefault)
            {
                var currentDefaultAddress = await _context.Addresss
                    .Where(a => a.UserId == request.UserId && a.IsDefault)
                    .ToListAsync();
                foreach (var addr in currentDefaultAddress)
                {
                    addr.IsDefault = false;
                    _context.Addresss.Update(addr);
                }
            }

            _mapper.Map(request, existingAddress);
            _context.Addresss.Update(existingAddress);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Address updated!";
            response.Data = _mapper.Map<AddressResponse>(existingAddress);

            return response;
        }

        public async Task<AddressListObjectResponse> GetByUserId(string userId)
        {
            AddressListObjectResponse response = new();

            var existingAddresses = await _context.Addresss
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (existingAddresses == null || !existingAddresses.Any())
            {
                throw new NotFoundException("Address not exists!");
            }

            response.StatusCode = ResponseCode.OK;
            response.Message = "Address retrieved";
            response.Data = _mapper.Map<List<AddressResponse>>(existingAddresses);

            return response;
        }

        public async Task<AddressObjectResponse> DeleteAddressAsync(int id, string userId)
        {
            AddressObjectResponse response = new();

            var address = await _context.Addresss.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == id);

            if (address == null)
            {
                throw new NotFoundException("Address not found!");
            }

            _context.Addresss.Remove(address);
            await _context.SaveChangesAsync();

            response.StatusCode = ResponseCode.OK;
            response.Message = "Address deleted!";

            return response;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthRequest request)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Google:ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TokenObjectResponse> GoogleLogin(ExternalAuthRequest request)
        {
            var response = new TokenObjectResponse();
            var payload = await VerifyGoogleToken(request);
            if (payload == null)
            {
                throw new BadRequestException("Invalid External Authentication.");
            };

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, "Customer"); // Add user to "Customer" role
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            if (user == null)
            {
                throw new BadRequestException("Invalid External Authentication.");
            }

            // Create a list of claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "Customer")
            };

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(claims),
                RefreshToken = GenerateRefreshToken()
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<FacebookUserResponse> VerifyFacebookToken(ExternalAuthRequest request)
        {
            var appId = _configuration["Facebook:AppId"];
            var appSecret = _configuration["Facebook:AppSecret"];
            var validationUri = $"https://graph.facebook.com/debug_token?input_token={request.IdToken}&access_token={appId}|{appSecret}";

            using var client = new HttpClient();
            var validationResponse = await client.GetAsync(validationUri);

            if (!validationResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to validate Facebook access token.");
            }

            var validationContent = await validationResponse.Content.ReadAsStringAsync();
            var validationResult = JsonConvert.DeserializeObject<dynamic>(validationContent);

            if (validationResult.data.app_id != appId)
            {
                throw new BadRequestException("Invalid Facebook App ID.");
            }

            var userUri = $"https://graph.facebook.com/v13.0/me?fields=id,name,email,first_name,last_name&access_token={request.IdToken}";
            var userResponse = await client.GetAsync(userUri);

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to fetch Facebook user data.");
            }

            var userContent = await userResponse.Content.ReadAsStringAsync();
            var fbUserData = JsonConvert.DeserializeObject<FacebookUserResponse>(userContent);

            return fbUserData;
        }

        public async Task<TokenObjectResponse> FacebookLogin(ExternalAuthRequest request)
        {
            var response = new TokenObjectResponse();

            var fbUserData = await VerifyFacebookToken(request);
            if (fbUserData == null)
            {
                throw new BadRequestException("Invalid Facebook access token.");
            }

            var user = await _userManager.FindByEmailAsync(fbUserData.Email);



            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = fbUserData.Email,
                    UserName = fbUserData.Email,
                    FirstName = fbUserData.First_Name,
                    LastName = fbUserData.Last_Name
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user: " + result.Errors.FirstOrDefault()?.Description);
                }
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            // Associate Facebook login with the user (assuming user already exists)
            var info = new UserLoginInfo(request.Provider, fbUserData.Id, request.Provider);
            await _userManager.AddLoginAsync(user, info);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "Customer")
            };

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(claims),
                RefreshToken = GenerateRefreshToken()
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }
    }
}
