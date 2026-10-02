using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task CreateUser()
    {
        Console.Write("Please enter your username: ");
        string? username = Console.ReadLine();
        
        Console.Write("Please enter your password: ");
        string? password = Console.ReadLine();
        
        User newUser = User.Create(username, password);

        try
        {
            User createdUser = await userRepository.AddUserAsync(newUser);

            Console.WriteLine("User Created Successfully:");
            Console.WriteLine($"Username: {createdUser.Username}");
            Console.WriteLine("You know your password right?");
            Console.WriteLine($"UserId: {createdUser.UserId}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}