using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartShop.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckout([FromBody] CheckoutRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _checkoutService.CreateCheckoutSessionAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("status/{sessionId}")]
        public async Task<IActionResult> GetCheckoutStatus(string sessionId)
        {
            var result = await _checkoutService.GetCheckoutStatusAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}