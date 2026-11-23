using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcUserRepository : IUserRepository
{
    private readonly AppContext ctx;

    public EfcUserRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        EntityEntry<User> entityEntry = await ctx.Users.AddAsync(user);
        await ctx.SaveChangesAsync();
        return entityEntry.Entity;
    }
    
    public async Task<User> GetSingleUserAsync(int userId)
    {
        User? user = await ctx.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException(
                $"User with ID '{userId}' does not exist.");
        }
        return user;
    }
    
    public IQueryable<User> GetAllUsersAsync()
    {
        return ctx.Users.AsQueryable();
    }
    
    public async Task DeleteUserAsync(int userId)
    {
        User? user = await ctx.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with id {userId} does not exist.");
        }

        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();
    }
    
    public async Task UpdateUserAsync(User user)
    {
        User? existingUser = await ctx.Users.FindAsync(user.UserId);
        if (existingUser == null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.UserId}' does not exist.");
        }
        existingUser.Username = user.Username;
        existingUser.Password = user.Password;
        await ctx.SaveChangesAsync();
    }
    
    public async Task<User?> GetUserByUsernameAndPasswordAsync(string? username, string? password)
    {
        return await ctx.Users
            .SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
    }
}