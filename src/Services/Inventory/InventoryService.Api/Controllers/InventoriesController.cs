using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all inventory items")]
        public async Task<IActionResult> GetAllInventoryItems()
        {
            var inventoryItems = await _inventoryService.GetAllInventoryItemsAsync();
            return Ok(inventoryItems);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get inventory item by ID")]
        public async Task<IActionResult> GetInventoryItemById(Guid id)
        {
            var inventoryItem = await _inventoryService.GetInventoryItemByIdAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            return Ok(inventoryItem);
        }

        [HttpGet("product/{productId:guid}")]
        [SwaggerOperation(Summary = "Get inventory item by product ID")]
        public async Task<IActionResult> GetInventoryItemByProductId(Guid productId)
        {
            var inventoryItem = await _inventoryService.GetInventoryItemByProductIdAsync(productId);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            return Ok(inventoryItem);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create inventory item")]
        public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemDto dto)
        {
            try
            {
                var createdInventoryItem = await _inventoryService.CreateInventoryItemAsync(dto);
                return CreatedAtAction(nameof(GetInventoryItemById), new { id = createdInventoryItem.Id }, createdInventoryItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update inventory item")]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] UpdateInventoryItemDto dto)
        {
            try
            {
                var updatedInventoryItem = await _inventoryService.UpdateInventoryItemAsync(id, dto);
                if (updatedInventoryItem == null)
                {
                    return NotFound();
                }
                return Ok(updatedInventoryItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete inventory item")]
        public async Task<IActionResult> DeleteInventoryItem(Guid id)
        {
            var deleted = await _inventoryService.DeleteInventoryItemAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
