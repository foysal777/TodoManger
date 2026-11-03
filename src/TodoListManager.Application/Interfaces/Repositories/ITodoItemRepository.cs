using TodoListManager.Application.Common;
using TodoListManager.Application.DTOs.Todo;
using TodoListManager.Domain.Entities;

namespace TodoListManager.Application.Interfaces.Repositories;

public interface ITodoItemRepository : IRepository<TodoItem>
{
    Task<TodoItem?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<TodoItem>> GetFilteredTodosAsync(TodoFilterParameters parameters, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TodoItem>> GetUpcomingTodosAsync(int days, CancellationToken cancellationToken = default);
    Task<TodoStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
