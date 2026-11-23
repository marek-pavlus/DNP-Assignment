using ApiContracts.Comments;
using ApiContracts.Users;

namespace ApiContracts.Posts;

public class PostDto
{
    public int PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int UserId { get; set; }

    public UserDto? Author { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}