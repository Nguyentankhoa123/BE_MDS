using MDS.Services;
using MDS.Services.DTO.Account;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private IAccountService _accountService;
        public AddressController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET api/<AddressController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _accountService.GetByUserId(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<AddressController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddressRequest request)
        {
            var result = await _accountService.CreateAddressAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<AddressController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AddressRequest request)
        {
            var result = await _accountService.UpdateAddressAsync(id, request);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE api/<AddressController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            var result = await _accountService.DeleteAddressAsync(id, userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
