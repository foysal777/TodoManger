using Microsoft.EntityFrameworkCore;
using TodoListManager.Domain.Entities;
using TodoListManager.Domain.Enums;

namespace TodoListManager.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Automatically create SQLite database and schema if not exists
        await context.Database.EnsureCreatedAsync();

        // Seed initial categories if none exist
        if (!await context.Categories.AnyAsync())
        {
            var workCategory = new Category
            {
                Name = "Work & Projects",
                Description = "Professional tasks, meetings, and project deadlines",
                ColorHex = "#3B82F6"
            };

            var personalCategory = new Category
            {
                Name = "Personal & Health",
                Description = "Personal goals, exercise, and health routines",
                ColorHex = "#10B981"
            };

            var studyCategory = new Category
            {
                Name = "Learning & Skills",
                Description = "Books, courses, and programming tutorials",
                ColorHex = "#8B5CF6"
            };

            var urgentCategory = new Category
            {
                Name = "Urgent Tasks",
                Description = "High priority items needing immediate attention",
                ColorHex = "#EF4444"
            };

            await context.Categories.AddRangeAsync(workCategory, personalCategory, studyCategory, urgentCategory);
            await context.SaveChangesAsync();

            // Seed initial todo items
            var todos = new List<TodoItem>
            {
                new()
                {
                    Title = "Set up ASP.NET Core Clean Architecture",
                    Description = "Structure Domain, Application, Infrastructure, and API layers with EF Core",
                    Priority = PriorityLevel.Urgent,
                    Status = TodoStatus.Completed,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    CategoryId = workCategory.Id,
                    DueDate = DateTime.UtcNow.AddDays(1)
                },
                new()
                {
                    Title = "Review Pull Requests and Sprint Backlog",
                    Description = "Review PRs submitted by team members before end of week",
                    Priority = PriorityLevel.High,
                    Status = TodoStatus.InProgress,
                    IsCompleted = false,
                    CategoryId = workCategory.Id,
                    DueDate = DateTime.UtcNow.AddDays(2)
                },
                new()
                {
                    Title = "Read Clean Architecture & Domain-Driven Design",
                    Description = "Study Chapter 4 on decoupled domain entities and repository patterns",
                    Priority = PriorityLevel.Medium,
                    Status = TodoStatus.Pending,
                    IsCompleted = false,
                    CategoryId = studyCategory.Id,
                    DueDate = DateTime.UtcNow.AddDays(5)
                },
                new()
                {
                    Title = "Morning 30-minute cardio and hydration",
                    Description = "Maintain consistent morning workout schedule",
                    Priority = PriorityLevel.Low,
                    Status = TodoStatus.Pending,
                    IsCompleted = false,
                    CategoryId = personalCategory.Id,
                    DueDate = DateTime.UtcNow.AddDays(1)
                }
            };

            await context.TodoItems.AddRangeAsync(todos);
            await context.SaveChangesAsync();
        }
    }
}
