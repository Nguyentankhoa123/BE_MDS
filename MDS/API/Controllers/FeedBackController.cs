using MDS.Services;
using MDS.Services.DTO.FeedBack;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        private readonly IFeedBackService _feedBackService;
        public FeedBackController(IFeedBackService feedBackService)
        {
            _feedBackService = feedBackService;
        }

        // GET api/<FeedBackController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _feedBackService.GetFeedbacksByDrugstore(id);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST api/<FeedBackController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedBackRequest request)
        {
            var result = await _feedBackService.CreateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var result = await _feedBackService.GetAllDrugstores();
        //    return Ok(result);
        //}


    }
}
