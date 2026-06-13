using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartShop.Controllers
{
    [ApiController]
    [Route("api/checkout")]

    // ⚠️ مؤقتًا هنشيل Authorize عشان نوقف redirect bug
    // [Authorize]

    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        [AllowAnonymous] // 👈 مهم جدًا
        public async Task<IActionResult> CreateCheckout()
        {
            var result = await _checkoutService.CreateCheckoutSessionAsync(null);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("status/{sessionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCheckoutStatus(string sessionId)
        {
            var result = await _checkoutService.GetCheckoutStatusAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}