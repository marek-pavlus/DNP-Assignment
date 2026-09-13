namespace Entities;

public class Comment
{
    public string Body { get; set; }
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    
    public Comment(string body, int postId, int userId)
    {
        Body = body;
        PostId = postId;
        UserId = userId;
    }
    
    private Comment()
    {
        
    } 
    // Static factory method
    public static Comment Create(string body, int postId, int userId)
    {
        return new Comment
        {
            Body = body,
            PostId = postId,
            UserId = userId
        };
    }
}

