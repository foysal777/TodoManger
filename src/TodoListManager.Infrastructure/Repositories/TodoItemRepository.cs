using Microsoft.EntityFrameworkCore;
using TodoListManager.Application.Common;
using TodoListManager.Application.DTOs.Todo;
using TodoListManager.Application.Interfaces.Repositories;
using TodoListManager.Domain.Entities;
using TodoListManager.Domain.Enums;
using TodoListManager.Infrastructure.Data;

namespace TodoListManager.Infrastructure.Repositories;

public class TodoItemRepository : Repository<TodoItem>, ITodoItemRepository
{
    public TodoItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TodoItem?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<PagedResult<TodoItem>> GetFilteredTodosAsync(TodoFilterParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(t => t.Category).AsNoTracking().AsQueryable();

        // Filtering by search term
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(search) || 
                                     (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        // Filtering by category
        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == parameters.CategoryId.Value);
        }

        // Filtering by priority
        if (parameters.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == parameters.Priority.Value);
        }

        // Filtering by status
        if (parameters.Status.HasValue)
        {
            query = query.Where(t => t.Status == parameters.Status.Value);
        }

        // Filtering by completion
        if (parameters.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == parameters.IsCompleted.Value);
        }

        // Filtering by overdue
        if (parameters.IsOverdue.HasValue && parameters.IsOverdue.Value)
        {
            var now = DateTime.UtcNow;
            query = query.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < now);
        }

        // Sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "duedate" => parameters.SortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
            "priority" => parameters.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            "title" => parameters.SortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "status" => parameters.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            _ => parameters.SortDescending ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = parameters.PageNumber < 1 ? 1 : parameters.PageNumber;
        var pageSize = parameters.PageSize < 1 ? 10 : (parameters.PageSize > 100 ? 100 : parameters.PageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TodoItem>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IReadOnlyList<TodoItem>> GetUpcomingTodosAsync(int days, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(days);

        return await _dbSet
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate >= now && t.DueDate <= threshold)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var total = await _dbSet.CountAsync(cancellationToken);
        var completed = await _dbSet.CountAsync(t => t.IsCompleted, cancellationToken);
        var inProgress = await _dbSet.CountAsync(t => t.Status == TodoStatus.InProgress, cancellationToken);
        var pending = await _dbSet.CountAsync(t => t.Status == TodoStatus.Pending && !t.IsCompleted, cancellationToken);
        var overdue = await _dbSet.CountAsync(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < now, cancellationToken);

        var completionRate = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0;

        return new TodoStatsDto
        {
            TotalTodos = total,
            CompletedTodos = completed,
            PendingTodos = pending,
            InProgressTodos = inProgress,
            OverdueTodos = overdue,
            CompletionRate = completionRate
        };
    }
}
