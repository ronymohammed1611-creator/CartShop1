using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartShop.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IConfiguration _configuration;

        public CheckoutService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ── POST /api/checkout ──
        public async Task<CheckoutResponseDto> CreateCheckoutSessionAsync(CheckoutRequest request)
        {
            // 1. Validate request
            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "Cart is empty"
                };
            }

            // 2. Build Stripe line items
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in request.Items)
            {
                var unitAmountInCents = (long)(item.Price * 100);

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        UnitAmount = unitAmountInCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name
                        }
                    },
                    Quantity = item.Quantity
                });
            }

            // 3. Frontend URL
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",

                SuccessUrl = $"{frontendUrl}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{frontendUrl}/cart",

                Metadata = new Dictionary<string, string>
                {
                    { "source", "firebase_cart" }
                }
            };

            var service = new SessionService();
            Session session;

            try
            {
                session = await service.CreateAsync(options);
            }
            catch (Exception ex)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = $"Stripe error: {ex.Message}"
                };
            }

            return new CheckoutResponseDto
            {
                Success = true,
                Message = "Checkout session created successfully",
                SessionId = session.Id,
                CheckoutUrl = session.Url
            };
        }

        // ── GET /api/checkout/status/:sessionId ──
        public async Task<CheckoutStatusDto> GetCheckoutStatusAsync(string sessionId)
        {
            var service = new SessionService();

            try
            {
                var session = await service.GetAsync(sessionId);

                return new CheckoutStatusDto
                {
                    Success = true,
                    Message = "Status retrieved successfully",
                    SessionId = session.Id,
                    PaymentStatus = session.PaymentStatus,
                    SessionStatus = session.Status
                };
            }
            catch (Exception ex)
            {
                return new CheckoutStatusDto
                {
                    Success = false,
                    Message = $"Stripe error: {ex.Message}"
                };
            }
        }
    }
}