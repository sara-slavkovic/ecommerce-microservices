using CatalogService.Application.DTOs;
using CatalogService.Application.Enums;
using CatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, dto);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            return Ok(updatedCategory);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return result switch
            {
                DeleteCategoryResult.Success => NoContent(),
                DeleteCategoryResult.NotFound => NotFound(),
                DeleteCategoryResult.HasSubcategories => Conflict(new { message = "Category has subcategories and cannot be deleted." }),
                DeleteCategoryResult.HasProducts => Conflict(new { message = "Category has products and cannot be deleted." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred while deleting the category." })
            };
        }
    }
}
