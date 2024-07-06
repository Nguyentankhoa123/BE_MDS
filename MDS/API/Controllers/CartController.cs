using AutoMapper;
using MDS.Services;
using MDS.Shared.Database.DbContext;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        public CartController(AppDbContext context, IMapper mapper, ICartService cartService)
        {
            _context = context;
            _mapper = mapper;
            _cartService = cartService;
        }


        // GET api/<CartController>/5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string userId)
        {
            var result = await _cartService.GetAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<CartController>
        [HttpPost]
        public async Task<IActionResult> Post(string userId, int productId, int quantity)
        {
            var result = await _cartService.CreateAsync(userId, productId, quantity);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpDelete("Decrease")]
        public async Task<IActionResult> Decrerase([FromQuery] string userId, int productId, int quantity)
        {
            var result = await _cartService.DecreaseAsync(userId, productId, quantity);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Delete([FromQuery] string userId, int productId)
        {
            var result = await _cartService.RemoveAsync(userId, productId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("RemoveAll")]
        public async Task<IActionResult> Delete([FromQuery] string userId)
        {
            var result = await _cartService.RemoveAllAsync(userId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
