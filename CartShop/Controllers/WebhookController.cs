using CartShop.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CartShop.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;

        public WebhookController(IOrderService orderService, IConfiguration configuration)
        {
            _orderService = orderService;
            _configuration = configuration;
        }

        /// <summary>
        /// POST /api/webhook/stripe
        /// Stripe بيبعت هنا لما يتم الدفع
        /// مهم: لازم تضيف الـ WebhookSecret في appsettings
        /// </summary>
        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret =
                _configuration["Stripe:WebhookSecret"]
                ?? Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
                ?? Environment.GetEnvironmentVariable("Stripe__WebhookSecret");

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            // ── Handle checkout.session.completed ──
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                if (session == null)
                    return BadRequest(new { error = "Invalid session data" });

                // جيب الـ userId من الـ metadata اللي حطيناها وقت إنشاء الـ session
                session.Metadata.TryGetValue("userId", out var userId);

                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { error = "userId not found in session metadata" });

                // إنشئ الـ Order وامسح الـ Cart
                var success = await _orderService.CreateOrderFromCartAsync(userId, session.Id);

                if (!success)
                    return StatusCode(500, new { error = "Failed to create order" });
            }

            // Stripe بيتوقع 200 OK دايماً حتى لو الـ event مش متعامل معاه
            return Ok(new { received = true });
        }
    }
}
