using MDS.Services;
using MDS.Services.DTO.Category;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _categoryService.GetAllAsync(pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id, [FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _categoryService.GetProductsByCategoryId(id, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.CreateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
