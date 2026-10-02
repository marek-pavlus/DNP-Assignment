using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task DisplayPostDetailsAsync(int postId)
    {
        try
        {
            var post = await postRepository.GetSinglePostAsync(postId);
            var user = await userRepository.GetSingleUserAsync(post.UserId);

            Console.WriteLine($"User: {user.Username}");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Body: {post.Body}");

            var comments = commentRepository.GetManyComments()
                .Where(c => c.PostId == postId)
                .ToList();

            if (comments.Any())
            {
                Console.WriteLine("Comments:");

                foreach (var comment in comments)
                {
                    var commentUser =
                        await userRepository.GetSingleUserAsync(comment.UserId);

                    Console.WriteLine($"User: {commentUser.Username}");
                    Console.WriteLine($"- {comment.Body}");
                }
            }
            else
            {
                Console.WriteLine("No comments available.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}