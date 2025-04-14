using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        // Constructor injection
        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // GET api/comment/{postId}
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPostIdAsync(string postId)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            if (comments == null || !comments.Any())
            {
                return NotFound($"No comments found for PostId {postId}");
            }

            return Ok(comments);
        }

        // GET api/comment/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByUserIdAsync(string userId)
        {
            var comments = await _commentRepository.GetCommentsByUserIdAsync(userId);
            if (comments == null || !comments.Any())
            {
                return NotFound($"No comments found for UserId {userId}");
            }

            return Ok(comments);
        }

        // POST api/comment
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateCommentAsync([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest("Comment is null");
            }

            await _commentRepository.CreateAsync(comment);
            return CreatedAtAction(nameof(GetCommentsByPostIdAsync), new { postId = comment.PostId }, comment);
        }

        // PUT api/comment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommentAsync(string id, [FromBody] Comment comment)
        {
            if (comment == null || comment.PostId == null)
            {
                return BadRequest("Invalid comment data");
            }

            await _commentRepository.UpdateAsync(id, comment);
            return NoContent();
        }

        // DELETE api/comment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync(string id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound($"Comment with id {id} not found");
            }

            await _commentRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}
