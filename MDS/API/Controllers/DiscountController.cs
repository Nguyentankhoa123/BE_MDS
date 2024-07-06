using MDS.Services;
using MDS.Services.DTO.Discount;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private IDiscountService _discountService;
        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }


        // GET api/<DiscountController>/5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? id, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _discountService.GetAllAsync(id, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<DiscountController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DiscountRequest request)
        {
            var result = await _discountService.CreateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<DiscountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DiscountController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _discountService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
