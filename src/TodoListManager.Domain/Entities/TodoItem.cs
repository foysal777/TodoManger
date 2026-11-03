using TodoListManager.Domain.Common;
using TodoListManager.Domain.Enums;

namespace TodoListManager.Domain.Entities;

public class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    // Foreign Key
    public int? CategoryId { get; set; }
    // Navigation property
    public Category? Category { get; set; }
}
