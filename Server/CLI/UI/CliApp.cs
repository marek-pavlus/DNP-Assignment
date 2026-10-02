using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private IUserRepository UserRepository { get; }
    private ICommentRepository CommentRepository { get; }
    private IPostRepository PostRepository { get; }
    private User? CurrentUser { get; set; }

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        UserRepository = userRepository;
        CommentRepository = commentRepository;
        PostRepository = postRepository;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            if (CurrentUser == null)
            {
                Console.WriteLine("Welcome to the CLI App! Choose an option:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create User");
                Console.WriteLine("3. View All Posts");
                Console.WriteLine("4. View Post Details");
                Console.WriteLine("5. Exit");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await LoginAsync();
                        break;
                    case "2":
                        await CreateUserAsync();
                        break;
                    case "3":
                        DisplayPosts();
                        break;
                    case "4":
                        await ViewPostDetailsAsync();
                        break;
                    case "5":
                        Console.WriteLine("Exiting the app.");
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            else
            {

                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. View All Posts");
                Console.WriteLine("2. View Post Details");
                Console.WriteLine("3. Create a Post");
                Console.WriteLine("4. Delete a Post");
                Console.WriteLine("5. Add a Comment");
                Console.WriteLine("6. Logout");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        DisplayPosts();
                        break;
                    case "2":
                        await ViewPostDetailsAsync();
                        break;
                    case "3":
                        await CreatePostAsync();
                        break;
                    case "4":
                        await DeletePostAsync();
                        break;
                    case "5":
                        await AddCommentAsync();
                        break;
                    case "6":
                        CurrentUser = null;
                        Console.WriteLine("Logged out successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }

    private async Task LoginAsync()
    {
        Console.WriteLine("Enter your username:");
        string? username = Console.ReadLine();

        Console.WriteLine("Enter your password:");
        string? password = Console.ReadLine();

        var user = await UserRepository
            .GetUserByUsernameAndPasswordAsync(username, password);

        if (user != null)
        {
            CurrentUser = user;
            Console.WriteLine($"Login successful. Welcome, {user.Username}!");
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
            Console.WriteLine("Press Enter to return to the main menu.");
            Console.ReadLine();
        }
    }

    private async Task CreateUserAsync()
    {
        var createUserView = new CreateUserView(UserRepository);
        await createUserView.CreateUser();
        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }

    private async Task CreatePostAsync()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("You need to be logged in to create a post.");
            return;
        }

        var createPostView = new CreatePostView(PostRepository, CurrentUser);
        await createPostView.AddPostAsync();
        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }

    private async Task AddCommentAsync()
    {
        if (CurrentUser == null)
        {
            Console.WriteLine("You need to be logged in to add a comment.");
            return;
        }

        var createCommentView = new CreateCommentView(CommentRepository, PostRepository, CurrentUser);
        await createCommentView.CreateCommentAsync();
        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }

    private void DisplayPosts()
    {
        var listPostView = new ListPostView(PostRepository);
        listPostView.DisplayPosts();
        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }

    private async Task ViewPostDetailsAsync()
    {
        Console.WriteLine("Enter the Post ID:");
        if (int.TryParse(Console.ReadLine(), out int postId))
        {
            var singlePostView = new SinglePostView(PostRepository, CommentRepository, UserRepository);
            await singlePostView.DisplayPostDetailsAsync(postId);
        }
        else
        {
            Console.WriteLine("Invalid Post ID.");
        }
        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }
    
    private async Task DeletePostAsync()
    {
        Console.WriteLine("Enter the Post ID:");

        if (int.TryParse(Console.ReadLine(), out int postId))
        {
            var deletePostView = new DeletePostView(PostRepository);
            await deletePostView.DeletePostAsync(postId);
        }
        else
        {
            Console.WriteLine("Invalid Post ID.");
        }

        Console.WriteLine("Press Enter to return to the main menu.");
        Console.ReadLine();
    }
}