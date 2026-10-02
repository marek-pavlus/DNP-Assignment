using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }
    
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromBody] CreateCommentDto request)
    {
        try
        {
            await userRepository.GetSingleUserAsync(request.UserId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist.");
        }

        try
        {
            await postRepository.GetSinglePostAsync(request.PostId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"Post with ID '{request.PostId}' does not exist.");
        }

        Comment comment = new Comment(
            request.Body,
            request.PostId,
            request.UserId);

        Comment createdComment =
            await commentRepository.AddCommentAsync(comment);

        CommentDto commentDto = new CommentDto
        {
            CommentId = createdComment.CommentId,
            Body = createdComment.Body,
            PostId = createdComment.PostId,
            UserId = createdComment.UserId
        };

        return CreatedAtAction(
            nameof(GetSingleComment),
            new { id = createdComment.CommentId },
            commentDto);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleCommentAsync(id);

            CommentDto commentDto = new CommentDto
            {
                CommentId = comment.CommentId,
                Body = comment.Body,
                PostId = comment.PostId,
                UserId = comment.UserId
            };

            return Ok(commentDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID '{id}' not found.");
        }
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetManyComments(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepository.GetManyComments();

        if (userId.HasValue)
        {
            comments = comments.Where(comment =>
                comment.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            List<int> matchingUserIds = userRepository
                .GetManyUsers()
                .Where(user =>
                    user.Username.Contains(
                        username,
                        StringComparison.OrdinalIgnoreCase))
                .Select(user => user.UserId)
                .ToList();

            comments = comments.Where(comment =>
                matchingUserIds.Contains(comment.UserId));
        }

        if (postId.HasValue)
        {
            comments = comments.Where(comment =>
                comment.PostId == postId.Value);
        }

        IEnumerable<CommentDto> commentDtos =
            comments.Select(comment => new CommentDto
            {
                CommentId = comment.CommentId,
                Body = comment.Body,
                PostId = comment.PostId,
                UserId = comment.UserId
            });

        return Ok(commentDtos);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(
        [FromRoute] int id,
        [FromBody] UpdateCommentDto request)
    {
        try
        {
            Comment comment = await commentRepository.GetSingleCommentAsync(id);

            comment.Body = request.Body;

            await commentRepository.UpdateCommentAsync(comment);

            CommentDto commentDto = new CommentDto
            {
                CommentId = comment.CommentId,
                Body = comment.Body,
                PostId = comment.PostId,
                UserId = comment.UserId
            };

            return Ok(commentDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID '{id}' not found.");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepository.DeleteCommentAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with ID '{id}' not found.");
        }
    }
}