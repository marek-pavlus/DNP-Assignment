using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new List<User>();

    public UserInMemoryRepository()
    {
        _ = AddUserAsync(new User("trmo", "1234")).Result;
        _ = AddUserAsync(new User("mivi", "4321")).Result;
        _ = AddUserAsync(new User("jknr", "1243")).Result;
        _ = AddUserAsync(new User("alhe", "2143")).Result;
    }
    
    public Task<User> AddUserAsync(User user)
    {
        user.UserId = users.Any() ? users.Max(u => u.UserId) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }
    
    public Task UpdateUserAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.UserId == user.UserId);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with ID '{user.UserId}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }
    
    public Task DeleteUserAsync(int id)
    {
        User? postToRemove = users.SingleOrDefault(u => u.UserId == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        users.Remove(postToRemove);
        return Task.CompletedTask;
    }
    
    public Task<User> GetSingleUserAsync(int id)
    {
        User? user = users.SingleOrDefault(u => u.UserId == id);
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID {id} not found");
        }
        return Task.FromResult(user);
    }
    
    public IQueryable<User> GetManyUsers()
    {
        return users.AsQueryable();
    }
    
    public Task<User?> GetUserByUsernameAndPasswordAsync(string? username, string? password)
    {
        User? user = users.SingleOrDefault(u => u.Username == username && u.Password == password);
        return Task.FromResult(user);
    }
}