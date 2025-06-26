using Domain.Entytis;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _context;

    public UserRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<List<User>> GetAllActiveAsync()
    {
        return await _context.Users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersByMinAgeAsync(int minAge)
    {
        var cutoffDate = DateTime.Today.AddYears(-minAge);
        return await _context.Users
            .Where(u => u.Birthday.HasValue && u.Birthday.Value <= cutoffDate)
            .ToListAsync();
    }

    public async Task<bool> LoginExistsAsync(string login, Guid? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.Login == login);
        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}