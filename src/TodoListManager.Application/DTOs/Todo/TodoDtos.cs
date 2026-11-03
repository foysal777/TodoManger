using System.ComponentModel.DataAnnotations;
using TodoListManager.Application.DTOs.Category;
using TodoListManager.Domain.Enums;

namespace TodoListManager.Application.DTOs.Todo;

public class TodoItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; }
    public string PriorityName => Priority.ToString();
    public TodoStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTodoItemDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public int? CategoryId { get; set; }
}

public class UpdateTodoItemDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public PriorityLevel Priority { get; set; }

    public TodoStatus Status { get; set; }

    public bool IsCompleted { get; set; }

    public int? CategoryId { get; set; }
}

public class TodoFilterParameters
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public PriorityLevel? Priority { get; set; }
    public TodoStatus? Status { get; set; }
    public bool? IsCompleted { get; set; }
    public bool? IsOverdue { get; set; }
    public string? SortBy { get; set; } // "dueDate", "priority", "createdAt", "title"
    public bool SortDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class TodoStatsDto
{
    public int TotalTodos { get; set; }
    public int CompletedTodos { get; set; }
    public int PendingTodos { get; set; }
    public int InProgressTodos { get; set; }
    public int OverdueTodos { get; set; }
    public double CompletionRate { get; set; }
}
