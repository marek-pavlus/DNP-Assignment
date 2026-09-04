using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new List<User>();
    
    public Task<User> AddAsync(User user)
    {
        user.UserId = users.Any()
            ? users.Max(u => u.UserId) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }
    
    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.UserId == user.UserId);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.UserId}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(int id)
    {
        User? postToRemove = users.SingleOrDefault(u => u.UserId == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(postToRemove);
        return Task.CompletedTask;
    }
    
    public Task<User> GetSingleAsync(int id)
    {
        User? user = users.SingleOrDefault(u => u.UserId == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID {id} not found");
        }
        return Task.FromResult(user);
    }
    
    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }
}