using MDS.Services;
using MDS.Services.DTO.Comment;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        // GET: api/<CommentController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? parentId, int productId)
        {
            var result = await _commentService.GetCommentsByParentIdAsync(parentId, productId);
            return StatusCode((int)result.StatusCode, result);
        }


        // POST api/<CommentController>
        [HttpPost]
        public async Task<IActionResult> Post(CommentRequest request)
        {
            var result = await _commentService.CreateAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }


        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromBody] DeleteCommentRequest request)
        {
            var result = await _commentService.DeleteAsync(request.CommentId, request.ProductId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
