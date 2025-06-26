using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure
{
    public class UserManagementDbContextFactory : IDesignTimeDbContextFactory<UserManagementDbContext>
    {
        public UserManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserManagementDbContext>();

            // 👉 Укажи здесь свою строку подключения!
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=your_db;Username=your_user;Password=your_password");

            return new UserManagementDbContext(optionsBuilder.Options);
        }
    }
}