using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IndustrialAutomationDbContext _context;

    public UserRepository(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
