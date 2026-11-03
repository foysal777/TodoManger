using Microsoft.EntityFrameworkCore;
using TodoListManager.Application.Interfaces.Repositories;
using TodoListManager.Domain.Entities;
using TodoListManager.Infrastructure.Data;

namespace TodoListManager.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetWithTodosAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.TodoItems)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllWithItemCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.TodoItems)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(c => c.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
