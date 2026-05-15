using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartShop.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // الـ AI (Raspberry Pi) بيبعت هنا
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCartAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // الـ Frontend بيجيب الـ Cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.GetCartAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // تحديث الكمية — PUT /api/cart/items/{itemId}?quantity=3
        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateQuantity(int itemId, [FromQuery] int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.UpdateQuantityAsync(userId, itemId, quantity);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // حذف منتج — DELETE /api/cart/items/{itemId}
        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.RemoveItemAsync(userId, itemId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // مسح الـ Cart كله
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.ClearCartAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}