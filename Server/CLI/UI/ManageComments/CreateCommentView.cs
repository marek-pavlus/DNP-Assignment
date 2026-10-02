using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly User currentUser;

    public CreateCommentView(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        User currentUser)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.currentUser = currentUser;
    }

    public async Task CreateCommentAsync()
    {
        Console.WriteLine("Enter the Post ID:");

        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID.");
            return;
        }

        try
        {
            await postRepository.GetSinglePostAsync(postId);

            Console.WriteLine("Enter your comment:");
            string? commentText = Console.ReadLine();

            Comment newComment =
                Comment.Create(commentText, postId, currentUser.UserId);

            await commentRepository.AddCommentAsync(newComment);

            Console.WriteLine("Comment created successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}