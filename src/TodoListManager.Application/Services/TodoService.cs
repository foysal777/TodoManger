using TodoListManager.Application.Common;
using TodoListManager.Application.DTOs.Category;
using TodoListManager.Application.DTOs.Todo;
using TodoListManager.Application.Exceptions;
using TodoListManager.Application.Interfaces.Repositories;
using TodoListManager.Application.Interfaces.Services;
using TodoListManager.Domain.Entities;
using TodoListManager.Domain.Enums;

namespace TodoListManager.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoItemRepository _todoRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TodoService(ITodoItemRepository todoRepository, ICategoryRepository categoryRepository)
    {
        _todoRepository = todoRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<TodoItemDto>> GetTodosAsync(TodoFilterParameters parameters, CancellationToken cancellationToken = default)
    {
        var pagedTodos = await _todoRepository.GetFilteredTodosAsync(parameters, cancellationToken);
        var dtos = pagedTodos.Items.Select(MapToDto).ToList();
        return new PagedResult<TodoItemDto>(dtos, pagedTodos.TotalCount, pagedTodos.PageNumber, pagedTodos.PageSize);
    }

    public async Task<TodoItemDto> GetTodoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.GetWithCategoryAsync(id, cancellationToken);
        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), id);
        }

        return MapToDto(todo);
    }

    public async Task<TodoItemDto> CreateTodoAsync(CreateTodoItemDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId.Value);
            }
        }

        var todo = new TodoItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            Status = TodoStatus.Pending,
            IsCompleted = false,
            CategoryId = dto.CategoryId
        };

        var created = await _todoRepository.AddAsync(todo, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);

        // Fetch with category to populate relationship
        var fullTodo = await _todoRepository.GetWithCategoryAsync(created.Id, cancellationToken);
        return MapToDto(fullTodo ?? created);
    }

    public async Task<TodoItemDto> UpdateTodoAsync(int id, UpdateTodoItemDto dto, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.GetWithCategoryAsync(id, cancellationToken);
        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), id);
        }

        if (dto.CategoryId.HasValue && dto.CategoryId != todo.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId.Value);
            }
        }

        todo.Title = dto.Title.Trim();
        todo.Description = dto.Description?.Trim();
        todo.DueDate = dto.DueDate;
        todo.Priority = dto.Priority;
        todo.Status = dto.Status;
        todo.CategoryId = dto.CategoryId;

        if (dto.IsCompleted && !todo.IsCompleted)
        {
            todo.IsCompleted = true;
            todo.Status = TodoStatus.Completed;
            todo.CompletedAt = DateTime.UtcNow;
        }
        else if (!dto.IsCompleted && todo.IsCompleted)
        {
            todo.IsCompleted = false;
            todo.Status = dto.Status == TodoStatus.Completed ? TodoStatus.Pending : dto.Status;
            todo.CompletedAt = null;
        }

        await _todoRepository.UpdateAsync(todo, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);

        var updated = await _todoRepository.GetWithCategoryAsync(id, cancellationToken);
        return MapToDto(updated ?? todo);
    }

    public async Task<TodoItemDto> ToggleCompleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.GetWithCategoryAsync(id, cancellationToken);
        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), id);
        }

        todo.IsCompleted = !todo.IsCompleted;
        if (todo.IsCompleted)
        {
            todo.Status = TodoStatus.Completed;
            todo.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            todo.Status = TodoStatus.Pending;
            todo.CompletedAt = null;
        }

        await _todoRepository.UpdateAsync(todo, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(todo);
    }

    public async Task DeleteTodoAsync(int id, CancellationToken cancellationToken = default)
    {
        var todo = await _todoRepository.GetByIdAsync(id, cancellationToken);
        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), id);
        }

        await _todoRepository.DeleteAsync(todo, cancellationToken);
        await _todoRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetUpcomingTodosAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        var todos = await _todoRepository.GetUpcomingTodosAsync(days, cancellationToken);
        return todos.Select(MapToDto).ToList();
    }

    public async Task<TodoStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        return await _todoRepository.GetStatsAsync(cancellationToken);
    }

    private static TodoItemDto MapToDto(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        DueDate = item.DueDate,
        Priority = item.Priority,
        Status = item.Status,
        IsCompleted = item.IsCompleted,
        CompletedAt = item.CompletedAt,
        CategoryId = item.CategoryId,
        Category = item.Category != null ? new CategoryDto
        {
            Id = item.Category.Id,
            Name = item.Category.Name,
            Description = item.Category.Description,
            ColorHex = item.Category.ColorHex,
            CreatedAt = item.Category.CreatedAt
        } : null,
        CreatedAt = item.CreatedAt,
        UpdatedAt = item.UpdatedAt
    };
}
