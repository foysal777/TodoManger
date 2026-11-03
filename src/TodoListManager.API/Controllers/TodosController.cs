using Microsoft.AspNetCore.Mvc;
using TodoListManager.Application.Common;
using TodoListManager.Application.DTOs.Todo;
using TodoListManager.Application.Interfaces.Services;

namespace TodoListManager.API.Controllers;

public class TodosController : BaseApiController
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    /// <summary>
    /// Get paginated list of todos with optional filtering, search, and sorting
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TodoItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<TodoItemDto>>>> GetTodos([FromQuery] TodoFilterParameters parameters, CancellationToken cancellationToken)
    {
        var result = await _todoService.GetTodosAsync(parameters, cancellationToken);
        return OkResult(result, "Todos retrieved successfully");
    }

    /// <summary>
    /// Get dashboard statistics for todos
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<TodoStatsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TodoStatsDto>>> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _todoService.GetStatsAsync(cancellationToken);
        return OkResult(stats, "Todo statistics retrieved successfully");
    }

    /// <summary>
    /// Get upcoming todos due within the next N days
    /// </summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TodoItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TodoItemDto>>>> GetUpcoming([FromQuery] int days = 7, CancellationToken cancellationToken = default)
    {
        var todos = await _todoService.GetUpcomingTodosAsync(days, cancellationToken);
        return OkResult(todos, $"Upcoming todos for the next {days} days");
    }

    /// <summary>
    /// Get a specific todo item by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var todo = await _todoService.GetTodoByIdAsync(id, cancellationToken);
        return OkResult(todo);
    }

    /// <summary>
    /// Create a new todo item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TodoItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> Create([FromBody] CreateTodoItemDto dto, CancellationToken cancellationToken)
    {
        var created = await _todoService.CreateTodoAsync(dto, cancellationToken);
        return CreatedResult(nameof(GetById), new { id = created.Id }, created, "Todo item created successfully");
    }

    /// <summary>
    /// Update an existing todo item
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TodoItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> Update(int id, [FromBody] UpdateTodoItemDto dto, CancellationToken cancellationToken)
    {
        var updated = await _todoService.UpdateTodoAsync(id, dto, cancellationToken);
        return OkResult(updated, "Todo item updated successfully");
    }

    /// <summary>
    /// Toggle the completion status of a todo item
    /// </summary>
    [HttpPatch("{id:int}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<TodoItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TodoItemDto>>> ToggleComplete(int id, CancellationToken cancellationToken)
    {
        var toggled = await _todoService.ToggleCompleteAsync(id, cancellationToken);
        return OkResult(toggled, $"Todo item marked as {(toggled.IsCompleted ? "Completed" : "Pending")}");
    }

    /// <summary>
    /// Delete a todo item by ID
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _todoService.DeleteTodoAsync(id, cancellationToken);
        return OkResult(true, "Todo item deleted successfully");
    }
}
