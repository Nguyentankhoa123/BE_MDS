using MDS.Services;
using MDS.Services.DTO.Product;
using MDS.Shared.Core.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, int pageSize = 5)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<ProductController>
        [HttpPost("Medicine")]
        //[Authorize(Roles = Roles.Drugstore)]
        public async Task<IActionResult> PostMedicine([FromQuery] string userId, [FromBody] MedicineRequest request)
        {
            var result = await _productService.CreateMedicineAsync(userId, request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("NotMedicine")]
        //[Authorize(Roles = Roles.Drugstore)]
        public async Task<IActionResult> PostNotMedicine([FromQuery] string userId, [FromBody] NotMedicineRequest request)
        {
            var result = await _productService.CreateNotMedicineAsync(userId, request);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<ProductController>/5
        [HttpPut("Update/{id}")]
        [Authorize(Roles = Roles.Drugstore)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductRequest request)
        {
            var result = await _productService.UpdateAsync(id, request);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Drugstore)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProductDrug([FromQuery] string id, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _productService.GetDrugstoresForProduct(id, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);

        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? nameQuery, string? activeIngredientQuery, string? useQuery, string? brandQuery, string? dosageFormQuery, string? filterQuery, string? priceSortOrder, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _productService.SearchAsync(nameQuery, activeIngredientQuery, useQuery, brandQuery, dosageFormQuery, filterQuery, priceSortOrder, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
