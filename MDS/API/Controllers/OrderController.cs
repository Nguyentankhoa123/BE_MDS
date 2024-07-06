using MDS.Services;
using MDS.Services.DTO.Order;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _orderService.GetAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("OrderDetail")]
        public async Task<IActionResult> GetOrderDetail([FromQuery] string drugstoreId, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _orderService.GetOrderDetailsByDrugstoreAsync(drugstoreId, pageNumber, pageSize);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderRequest request, string userId, string? code, int addressId)
        {
            var result = await _orderService.CreateAsync(request, userId, code, addressId, HttpContext);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpPost("OrderDrugstore")]
        public async Task<IActionResult> PostTestOrder([FromBody] OrderRequest request, string userId, string? code, int addressId)
        {
            var result = await _orderService.TestOrderDrugstore(request, userId, code, addressId, HttpContext);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("TestJmetter")]
        public async Task<IActionResult> TestJmetter(int productId, int quantity)
        {
            var result = await _orderService.TestOrderAsync(productId, quantity);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("TestOrderNoRedis")]
        public async Task<IActionResult> TestOrderNoRedis(int productId, int quantity)
        {
            var result = await _orderService.TestOrderNoReisAsync(productId, quantity);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
