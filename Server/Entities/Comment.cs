namespace Entities;

public class Comment
{
    public string Body { get; set; }
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
}