using MDS.Services;
using MDS.Services.DTO.Brand;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        // GET: api/<BrandController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _brandService.GetAllAsync(pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id, [FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _brandService.GetProductsByBrandId(id, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        // GET api/<BrandController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _brandService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<BrandController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BrandRequest request)
        {
            var result = await _brandService.CreateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }


        // DELETE api/<BrandController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
