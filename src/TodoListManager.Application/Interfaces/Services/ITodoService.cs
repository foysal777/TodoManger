using TodoListManager.Application.Common;
using TodoListManager.Application.DTOs.Todo;

namespace TodoListManager.Application.Interfaces.Services;

public interface ITodoService
{
    Task<PagedResult<TodoItemDto>> GetTodosAsync(TodoFilterParameters parameters, CancellationToken cancellationToken = default);
    Task<TodoItemDto> GetTodoByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TodoItemDto> CreateTodoAsync(CreateTodoItemDto dto, CancellationToken cancellationToken = default);
    Task<TodoItemDto> UpdateTodoAsync(int id, UpdateTodoItemDto dto, CancellationToken cancellationToken = default);
    Task<TodoItemDto> ToggleCompleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteTodoAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TodoItemDto>> GetUpcomingTodosAsync(int days = 7, CancellationToken cancellationToken = default);
    Task<TodoStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
