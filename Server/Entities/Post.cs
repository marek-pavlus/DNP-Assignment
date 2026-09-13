namespace Entities;

public class Post
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    
    public Post(string title, string body, int userId)
    {
        Title = title;
        Body = body;
        UserId = userId;
    }
    
    public static Post Create(string title, string body, int userId)
    {
        return new Post(title, body, userId)
        {
            Title = title,
            Body = body,
            UserId = userId
        };
    }
}