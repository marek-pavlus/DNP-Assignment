using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public PostsController(
        IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request)
    {
        try
        {
            await userRepository.GetSingleUserAsync(request.UserId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist.");
        }

        Post post = new Post(
            request.Title,
            request.Body,
            request.UserId);

        Post createdPost = await postRepository.AddPostAsync(post);

        PostDto postDto = new PostDto
        {
            PostId = createdPost.PostId,
            Title = createdPost.Title,
            Body = createdPost.Body,
            UserId = createdPost.UserId
        };

        return CreatedAtAction(
            nameof(GetSinglePost),
            new { id = createdPost.PostId },
            postDto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSinglePost([FromRoute] int id)
    {
        try
        {
            Post post = await postRepository.GetSinglePostAsync(id);

            PostDto postDto = new PostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            };

            return Ok(postDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with ID '{id}' not found.");
        }
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetManyPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        IQueryable<Post> posts = postRepository.GetManyPosts();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(post =>
                post.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(post => post.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            List<int> matchingUserIds = userRepository
                .GetManyUsers()
                .Where(user =>
                    user.Username.Contains(username, StringComparison.OrdinalIgnoreCase))
                .Select(user => user.UserId)
                .ToList();

            posts = posts.Where(post =>
                matchingUserIds.Contains(post.UserId));
        }

        IEnumerable<PostDto> postDtos = posts.Select(post => new PostDto
        {
            PostId = post.PostId,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId
        });

        return Ok(postDtos);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> UpdatePost(
        [FromRoute] int id,
        [FromBody] UpdatePostDto request)
    {
        try
        {
            Post post = await postRepository.GetSinglePostAsync(id);

            post.Title = request.Title;
            post.Body = request.Body;

            await postRepository.UpdatePostAsync(post);

            PostDto postDto = new PostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            };

            return Ok(postDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with ID '{id}' not found.");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepository.DeletePostAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with ID '{id}' not found.");
        }
    }
}