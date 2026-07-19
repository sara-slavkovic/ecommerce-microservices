using InventoryService.Api.Filters;
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

        // This endpoint should only be called by CatalogService, so that inventory item cannot be created without a valid product
        [HttpPost("internal")]
        [InternalApiKey]
        [SwaggerOperation(Summary = "Create inventory item")]
        public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemDto dto)
        {
            var createdInventoryItem = await _inventoryService.CreateInventoryItemAsync(dto);
            return CreatedAtAction(nameof(GetInventoryItemById), new { id = createdInventoryItem.Id }, createdInventoryItem);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete inventory item")]
        public async Task<IActionResult> DeleteInventoryItem(Guid id)
        {
            await _inventoryService.DeleteInventoryItemAsync(id);
            return NoContent();
        }

        [HttpDelete("product/{productId:guid}")]
        [SwaggerOperation(Summary = "Delete inventory item by product ID")]
        public async Task<IActionResult> DeleteInventoryItemByProductId(Guid productId)
        {
            await _inventoryService.DeleteInventoryItemByProductIdAsync(productId);
            return NoContent();
        }

        [HttpPost("reserve")]
        [SwaggerOperation(Summary = "Reserve stock")]
        public async Task<IActionResult> ReserveStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ReserveInventoryAsync(dto);
            return Ok(result);
        }

        [HttpPost("release")]
        [SwaggerOperation(Summary = "Release reserved stock")]
        public async Task<IActionResult> ReleaseStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ReleaseInventoryAsync(dto);
            return Ok(result);
        }

        [HttpPost("confirm")]
        [SwaggerOperation(Summary = "Confirm stock deduction")]
        public async Task<IActionResult> ConfirmStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ConfirmDeductionAsync(dto);
            return Ok(result);
        }

        [HttpPost("restock")]
        [SwaggerOperation(Summary = "Restock inventory")]
        public async Task<IActionResult> RestockInventory([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.RestockInventoryAsync(dto);
            return Ok(result);
        }
    }
}
