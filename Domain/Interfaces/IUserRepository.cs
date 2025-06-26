using Domain.Entytis;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAsync(string login);
    Task<List<User>> GetAllActiveAsync();
    Task<List<User>> GetUsersByMinAgeAsync(int minAge);
    Task<bool> LoginExistsAsync(string login, Guid? excludeUserId = null);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}