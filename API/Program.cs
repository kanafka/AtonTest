
using Domain.Entytis;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Database initialization
await InitializeDatabaseAsync(app);

Console.WriteLine("Starting User Management API...");
Console.WriteLine("Swagger UI available at: https://localhost:5001/");
builder.WebHost.UseUrls("http://*:5000", "http://*:5001");

app.Run();

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    
    try
    {
        Console.WriteLine("Initializing database...");
        
        // Применяем все миграции
        await context.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully!");
        
        // Проверяем, существует ли админ
        var adminExists = await userRepository.GetByLoginAsync("Admin");
        if (adminExists == null)
        {
            Console.WriteLine("Creating default admin user...");
            var admin = new User("Admin", "Admin123", "Administrator", Gender.Unknown, null, true, "System");
            await userRepository.AddAsync(admin);
            await userRepository.SaveChangesAsync();
            
            Console.WriteLine("Admin user created successfully!");
            Console.WriteLine("Login: Admin");
            Console.WriteLine("Password: Admin123");
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        Console.WriteLine("Please ensure PostgreSQL is running and connection string is correct.");
        throw;
    }
}