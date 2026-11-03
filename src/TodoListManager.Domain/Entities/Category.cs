using TodoListManager.Domain.Common;

namespace TodoListManager.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ColorHex { get; set; } = "#3B82F6"; // Default modern blue

    // Navigation property
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
