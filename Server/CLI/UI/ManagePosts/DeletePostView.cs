using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IPostRepository postRepository;

    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task DeletePostAsync()
    {
        Console.WriteLine("Enter the ID of the post you want to delete:");
        if (int.TryParse(Console.ReadLine(), out int postId))
        {
            var post = await postRepository.GetSinglePostAsync(postId);
            if (post == null)
            {
                Console.WriteLine("Post not found.");
                return;
            }

            Console.WriteLine($"Are you sure you want to delete the post titled '{post.Title}'? (y/n)");
            var confirmation = Console.ReadLine();
            if (confirmation?.ToLower() == "y")
            {
                await postRepository.DeletePostAsync(postId);
                Console.WriteLine("Post deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion canceled.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid post ID.");
        }
    }
}