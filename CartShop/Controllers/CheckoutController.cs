using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// ينشئ Stripe Checkout Session من الـ Cart الحالي
        /// يرجع CheckoutUrl — الـ Frontend يوجّه اليوزر إليها
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCheckout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _checkoutService.CreateCheckoutSessionAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// يتحقق من حالة الـ payment بعد الـ redirect من Stripe
        /// </summary>
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
