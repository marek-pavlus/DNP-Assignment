using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new List<Comment>();

    public CommentInMemoryRepository()
    {
        _ = AddCommentAsync(new Comment("Cats are great!", 1, 1)).Result;
        _ = AddCommentAsync(new Comment("So true!", 1, 2)).Result;
        _ = AddCommentAsync(new Comment("They're just so fluffy", 1, 2)).Result;
        _ = AddCommentAsync(new Comment("Mine's hairless!", 1, 1)).Result;
        _ = AddCommentAsync(new Comment("Is it sick?!", 1, 4)).Result;
        
        _ = AddCommentAsync(new Comment("Cats are still great!", 2, 2)).Result;
        _ = AddCommentAsync(new Comment("Man, mine just spat out a dead mouse :(", 2, 3)).Result;
        _ = AddCommentAsync(new Comment("That's a compliment",2, 2)).Result;
        _ = AddCommentAsync(new Comment("No rats around my house!", 2, 1)).Result;

        _ = AddCommentAsync(new Comment("#FIRST", 3, 1)).Result;
        _ = AddCommentAsync(new Comment("They're just so happy and loving", 3, 2)).Result;
        _ = AddCommentAsync(new Comment("Too noisy for me!", 3, 4)).Result;
        _ = AddCommentAsync(new Comment("Uhhh, no?? Cats forever", 3, 4)).Result;
        
        _ = AddCommentAsync(new Comment("Weather is just the greatest!", 4, 4)).Result;
        _ = AddCommentAsync(new Comment("Not today! It's raining :(", 4, 3)).Result;
        _ = AddCommentAsync(new Comment("Rain just smells so nice", 4, 4)).Result;
        _ = AddCommentAsync(new Comment("Weirdo :O", 4, 1)).Result;
        
        _ = AddCommentAsync(new Comment("HELP!?", 5, 1)).Result;
        _ = AddCommentAsync(new Comment("How do I even do anything?", 5, 1)).Result;
        _ = AddCommentAsync(new Comment("I don't understand async", 5, 2)).Result;
        _ = AddCommentAsync(new Comment("What do you need help with?", 5, 3)).Result;
        
        _ = AddCommentAsync(new Comment("Eh, what? Your carpet?", 6, 2)).Result;
        _ = AddCommentAsync(new Comment("I like my Mowinator3000, it just works", 6, 4)).Result;
        _ = AddCommentAsync(new Comment("It just grows out of control!", 6, 3)).Result;
        _ = AddCommentAsync(new Comment("What color is your carpet?", 6, 1)).Result;
    }

public Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CommentId = comments.Any()
            ? comments.Max(c => c.CommentId) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }
    
    public Task UpdateCommentAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.CommentId == comment.CommentId);
        if (existingComment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.CommentId}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask;
    }
    
    public Task DeleteCommentAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.CommentId == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }
    
    public Task<Comment> GetSingleCommentAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.CommentId == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }
        return Task.FromResult(comment);
    }
    
    public IQueryable<Comment> GetManyComments()
    {
        return comments.AsQueryable();
    }
}