using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new List<Post>();

    public PostInMemoryRepository()
    {
        _ = AddPostAsync(new Post("Cat discussion", "Cats are pretty neat, sometimes.", 1)).Result;
        _ = AddPostAsync(new Post("Cat discussion 2", "Cat dropped a dead bird in my bed. No longer neat.", 1)).Result;
        _ = AddPostAsync(new Post("Dog discussion", "Dogs are just far superior to cats. EOD.", 3)).Result;
        _ = AddPostAsync(new Post("Weather?", "So, does anyone else like weather?", 2)).Result;
        _ = AddPostAsync(new Post("DNP QA", "This post is for DNP discussions, or if you need help with stuff.", 4)).Result;
        _ = AddPostAsync(new Post("Best lawn mower?", "What's the best lawn mower robot to mow my living room carpet?", 3)).Result;
    }
    
    public Task<Post> AddPostAsync(Post post)
    {
        post.PostId = posts.Any()
            ? posts.Max(p => p.PostId) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }
    
    public Task UpdatePostAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.PostId == post.PostId);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with ID '{post.PostId}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }
    
    public Task DeletePostAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.PostId == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }
    
    public Task<Post> GetSinglePostAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.PostId == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }
        return Task.FromResult(post);
    }
    
    public IQueryable<Post> GetManyPosts()
    {
        return posts.AsQueryable();
    }
}