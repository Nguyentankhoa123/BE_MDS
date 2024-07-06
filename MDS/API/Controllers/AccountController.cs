using MDS.Model.Entity;
using MDS.Services;
using MDS.Services.DTO.Account;
using MDS.Services.DTO.ExternalAuth;
using MDS.Shared.Core.Exceptions;
using MDS.Shared.Core.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private UserManager<ApplicationUser> _userManager;
        public AccountController(IAccountService accountService, UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }
        // GET: api/<AccountController>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<AccountController>
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AccountRegisterRequest request)
        {
            var result = await _accountService.RegisterAsync(request, Roles.Admin);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("RegisterDrugstore")]
        public async Task<IActionResult> RegisterDrugstore([FromBody] AccountRegisterRequest request)
        {
            var result = await _accountService.RegisterAsync(request, Roles.Drugstore);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] AccountRegisterRequest request)
        {
            var result = await _accountService.RegisterAsync(request, Roles.Customer);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            var reuslt = await _userManager.DeleteAsync(user);

            if (reuslt.Succeeded)
            {
                return StatusCode(200, "User deleted successfully");
            }
            else
            {
                return StatusCode(400, "Error deleting user");
            }

        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(TokenRequest request)
        {
            var result = await _accountService.GetRefreshTokenAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Login-Google")]
        public async Task<IActionResult> LoginGoogle([FromBody] ExternalAuthRequest request)
        {
            var result = await _accountService.GoogleLogin(request);
            return Ok(result);
        }

        [HttpPost("Login-Facebook")]
        public async Task<IActionResult> LoginFacebook([FromBody] ExternalAuthRequest request)
        {
            var result = await _accountService.FacebookLogin(request);
            return Ok(result);
        }
    }
}
