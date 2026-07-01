using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CartService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Get cart by user ID")]
        public async Task<IActionResult> GetCartByUserId(Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }

        [HttpPost("items")]
        [SwaggerOperation(Summary = "Add item to cart")]
        public async Task<IActionResult> AddItemToCart([FromBody] CreateCartItemDto dto)
        {
            try
            {
                var cart = await _cartService.AddItemToCartAsync(dto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("items")]
        [SwaggerOperation(Summary = "Update cart item quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityDto dto)
        {
            try
            {
                var cart = await _cartService.UpdateCartItemQuantityAsync(dto);
                if (cart == null)
                {
                    return NotFound();
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("user/{userId:guid}/items/{productId:guid}")]
        [SwaggerOperation(Summary = "Remove item from cart")]
        public async Task<IActionResult> RemoveCartItem(Guid userId, Guid productId)
        {
            var removed = await _cartService.RemoveCartItemAsync(userId, productId);
            if (!removed)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("user/{userId:guid}")]
        [SwaggerOperation(Summary = "Delete cart")]
        public async Task<IActionResult> DeleteCart(Guid userId)
        {
            var deleted = await _cartService.DeleteCartAsync(userId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}