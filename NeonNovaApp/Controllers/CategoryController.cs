using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequestDto dto)
        {
            var category = await _categoryService.AddCategoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id },category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryRequestDto dto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts(int id)
        {
            var products = await _categoryService.GetProductsByCategoryAsync(id);
            return Ok(products);
        }
    }
}
