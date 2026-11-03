using TodoListManager.Domain.Entities;

namespace TodoListManager.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetWithTodosAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllWithItemCountAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
