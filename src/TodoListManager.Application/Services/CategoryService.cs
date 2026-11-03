using TodoListManager.Application.DTOs.Category;
using TodoListManager.Application.Exceptions;
using TodoListManager.Application.Interfaces.Repositories;
using TodoListManager.Application.Interfaces.Services;
using TodoListManager.Domain.Entities;

namespace TodoListManager.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllWithItemCountAsync(cancellationToken);
        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetWithTodosAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        return MapToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        if (await _categoryRepository.ExistsByNameAsync(dto.Name.Trim(), cancellationToken: cancellationToken))
        {
            throw new BadRequestException($"Category with name '{dto.Name}' already exists.");
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            ColorHex = dto.ColorHex
        };

        var created = await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(created);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        if (await _categoryRepository.ExistsByNameAsync(dto.Name.Trim(), excludeId: id, cancellationToken: cancellationToken))
        {
            throw new BadRequestException($"Category with name '{dto.Name}' already exists.");
        }

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();
        category.ColorHex = dto.ColorHex;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException(nameof(Category), id);
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }

    private static CategoryDto MapToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        ColorHex = c.ColorHex,
        TodoCount = c.TodoItems?.Count ?? 0,
        CreatedAt = c.CreatedAt
    };
}
