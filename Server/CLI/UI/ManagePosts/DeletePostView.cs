using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IPostRepository postRepository;

    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task DeletePostAsync(int postId)
    {
        try
        {
            var post = await postRepository.GetSinglePostAsync(postId);

            Console.WriteLine($"Are you sure you want to delete '{post.Title}'? (y/n)");
            string? confirmation = Console.ReadLine();

            if (confirmation?.ToLower() == "y")
            {
                await postRepository.DeletePostAsync(postId);
                Console.WriteLine("Post deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}