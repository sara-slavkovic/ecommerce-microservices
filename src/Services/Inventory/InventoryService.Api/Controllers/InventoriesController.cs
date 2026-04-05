using InventoryService.Application.DTOs;
using InventoryService.Application.Enums;
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

        [HttpDelete("product/{productId:guid}")]
        [SwaggerOperation(Summary = "Delete inventory item by product ID")]
        public async Task<IActionResult> DeleteInventoryItemByProductId(Guid productId)
        {
            var deleted = await _inventoryService.DeleteInventoryItemByProductIdAsync(productId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("reserve")]
        [SwaggerOperation(Summary = "Reserve stock")]
        public async Task<IActionResult> ReserveStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ReserveInventoryAsync(dto);

            return result switch
            {
                ReserveInventoryResult.Success => Ok(),
                ReserveInventoryResult.InventoryItemNotFound => NotFound(new { message = "Inventory item not found." }),
                ReserveInventoryResult.InvalidQuantity => BadRequest(new { message = "Quantity must be greater than zero." }),
                ReserveInventoryResult.InsufficientAvailableQuantity => Conflict(new { message = "Not enough available stock." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred while reserving stock." })
            };
        }

        [HttpPost("release")]
        [SwaggerOperation(Summary = "Release reserved stock")]
        public async Task<IActionResult> ReleaseStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ReleaseInventoryAsync(dto);

            return result switch
            {
                ReleaseInventoryResult.Success => Ok(),
                ReleaseInventoryResult.InventoryItemNotFound => NotFound(new { message = "Inventory item not found." }),
                ReleaseInventoryResult.InvalidQuantity => BadRequest(new { message = "Quantity must be greater than zero." }),
                ReleaseInventoryResult.InsufficientReservedQuantity => Conflict(new { message = "Not enough reserved stock to release." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred while releasing stock." })
            };
        }

        [HttpPost("confirm")]
        [SwaggerOperation(Summary = "Confirm stock deduction")]
        public async Task<IActionResult> ConfirmStock([FromBody] ChangeInventoryQuantityDto dto)
        {
            var result = await _inventoryService.ConfirmDeductionAsync(dto);

            return result switch
            {
                ConfirmDeductionResult.Success => Ok(),
                ConfirmDeductionResult.InventoryItemNotFound => NotFound(new { message = "Inventory item not found." }),
                ConfirmDeductionResult.InvalidQuantity => BadRequest(new { message = "Quantity must be greater than zero." }),
                ConfirmDeductionResult.InsufficientReservedQuantity => Conflict(new { message = "Not enough reserved stock to confirm deduction." }),
                _ => StatusCode(500, new { message = "An unexpected error occurred while confirming stock deduction." })
            };

        }
    }
}
