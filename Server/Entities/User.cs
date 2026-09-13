namespace Entities;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int UserId { get; set; }
    
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

    private User()
    {
        
    }
    
    public static User Create(string username, string password)
    {
        return new User
        {
            Username = username,
            Password = password
        };
    }
}