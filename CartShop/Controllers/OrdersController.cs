using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartShop.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// GET /api/orders
        /// كل orders اليوزر المسجل دخول
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetOrdersAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// GET /api/orders/{orderId}
        /// تفاصيل order معين
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetOrderByIdAsync(userId, orderId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
