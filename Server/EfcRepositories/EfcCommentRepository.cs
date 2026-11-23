using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository
{
    private readonly AppContext ctx;

    public EfcCommentRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    
    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        EntityEntry<Comment> entityEntry = await ctx.Comments.AddAsync(comment);
        await ctx.SaveChangesAsync();
        return entityEntry.Entity;
    }
    
    public async Task<Comment> GetSingleCommentAsync(int commentId)
    {
        Comment? comment = await ctx.Comments.FindAsync(commentId);
        if (comment == null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{commentId}' does not exist.");
        }
        return comment;
    }

    public IQueryable<Comment> GetAllCommentsAsync()
    {
        return ctx.Comments.AsQueryable();
    }
    
    public async Task DeleteCommentAsync(int commentId)
    {
        Comment? comment = await ctx.Comments.SingleOrDefaultAsync(c => c.CommentId == commentId);
        if (comment == null)
        {
            throw new InvalidOperationException($"Comment with id {commentId} does not exist.");
        }

        ctx.Comments.Remove(comment);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        if (!(await ctx.Comments.AnyAsync(c => c.CommentId == comment.CommentId)))
        {
            throw new InvalidOperationException($"Comment with id {comment.CommentId} does not exist.");
        }

        ctx.Comments.Update(comment);
        await ctx.SaveChangesAsync();
    }
}