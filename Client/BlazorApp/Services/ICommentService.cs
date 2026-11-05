using ApiContracts.Comments;

namespace BlazorApp.Services;

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(CreateCommentDto request);
    Task<IEnumerable<CommentDto>> GetCommentsAsync(int postId);
}