using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _repository;

    public CategoryService(IMapper mapper, ICategoryRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetAllAsync() =>
        _mapper.Map<List<CategoryDto>>(await _repository.GetAllAsync());

    public async Task<CategoryDto> GetByIdAsync(int id) => _mapper.Map<CategoryDto>(await _repository.GetByIdAsync(id));

    public async Task<CategoryDto> AddCategoryAsync(CreateCategoryRequestDto category)
    {
        var newCategory = await _repository.AddAsync(_mapper.Map<Category>(category));
        return _mapper.Map<CategoryDto>(newCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto category)
    {
        var categoryDb = await _repository.GetByIdAsync(id);
        if (categoryDb == null)
            throw new KeyNotFoundException($"Categoria con ID {id} no encontrado");
    
        _mapper.Map(category, categoryDb);
    
        var categoryUpdated = await _repository.UpdateAsync(categoryDb);
        return _mapper.Map<CategoryDto>(categoryUpdated);
    }

    public async Task<List<ProductResponseDTO>> GetProductsByCategoryAsync(int category)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}