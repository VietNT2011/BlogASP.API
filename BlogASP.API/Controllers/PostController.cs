using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;

        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }

        // GET: api/Post/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            return Ok(post);
        }

        // POST: api/Post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _postRepository.CreateAsync(post);
            return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, post);
        }

        // PUT: api/Post/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, [FromBody] Post updatedPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPost = await _postRepository.GetByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            await _postRepository.UpdateAsync(id, updatedPost);
            return NoContent();
        }

        // DELETE: api/Post/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var existingPost = await _postRepository.GetByIdAsync(id);

            if (existingPost == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            await _postRepository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Post/ByCategory/{categoryId}
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategory(string categoryId)
        {
            var posts = await _postRepository.GetPostsByCategoryIdAsync(categoryId);
            return Ok(posts);
        }

        // GET: api/Post/ByAuthor/{authorId}
        [HttpGet("ByAuthor/{authorId}")]
        public async Task<IActionResult> GetPostsByAuthor(string authorId)
        {
            var posts = await _postRepository.GetPostsByAuthorIdAsync(authorId);
            return Ok(posts);
        }
    }
}
