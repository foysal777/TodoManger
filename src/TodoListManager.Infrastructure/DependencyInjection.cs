using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoListManager.Application.Interfaces.Repositories;
using TodoListManager.Application.Interfaces.Services;
using TodoListManager.Application.Services;
using TodoListManager.Infrastructure.Data;
using TodoListManager.Infrastructure.Repositories;

namespace TodoListManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? "Data Source=todolist.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Application Services
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
