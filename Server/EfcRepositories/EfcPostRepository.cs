using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcPostRepository : IPostRepository
{
    private readonly AppContext ctx;

    public EfcPostRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    
    public async Task<Post> AddPostAsync(Post post)
    {
        EntityEntry<Post> entityEntry = await ctx.Posts.AddAsync(post);
        await ctx.SaveChangesAsync();
        return entityEntry.Entity;
    }
    
    public async Task<Post> GetSinglePostAsync(int postId)
    {
        Post? post = await ctx.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{postId}' does not exist.");
        }
        return post;
    }
    
    public IQueryable<Post> GetAllPostsAsync()
    {
        return ctx.Posts.AsQueryable();
    }
    
    public async Task DeletePostAsync(int postId)
    {
        Post? post = await ctx.Posts.SingleOrDefaultAsync(p => p.PostId == postId);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with id {postId} does not exist.");
        }

        ctx.Posts.Remove(post);
        await ctx.SaveChangesAsync();
    }
    
    public async Task UpdatePostAsync(Post post)
    {
        if (!(await ctx.Posts.AnyAsync(p => p.PostId == post.PostId)))
        {
            throw new InvalidOperationException($"Post with id {post.PostId} does not exist.");
        }

        ctx.Posts.Update(post);
        await ctx.SaveChangesAsync();
    }
}