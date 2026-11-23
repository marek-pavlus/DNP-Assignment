using ApiContracts.Comments;
using ApiContracts.Posts;
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Microsoft.EntityFrameworkCore;

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
            nameof(GetPost),
            new { id = createdPost.PostId },
            postDto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost(
        [FromRoute] int id,
        [FromQuery] bool includeAuthor,
        [FromQuery] bool includeComments)
    {
        IQueryable<Post> query = postRepository
            .GetAllPostsAsync()
            .Where(p => p.PostId == id);

        if (includeAuthor)
            query = query.Include(p => p.User);

        if (includeComments)
            query = query.Include(p => p.Comments);

        PostDto? dto = await query.Select(post => new PostDto
        {
            PostId = post.PostId,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,

            Author = includeAuthor
                ? new UserDto
                {
                    UserId = post.User.UserId,
                    Username = post.User.Username
                }
                : null,

            Comments = includeComments
                ? post.Comments.Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    Body = c.Body,
                    PostId = c.PostId,
                    UserId = c.UserId
                }).ToList()
                : new List<CommentDto>()
        }).FirstOrDefaultAsync();

        return dto == null ? NotFound() : Ok(dto);
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