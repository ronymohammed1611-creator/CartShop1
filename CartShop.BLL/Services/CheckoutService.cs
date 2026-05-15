using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using CartShop.DAL.Model.Enums;
using CartShop.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartShop.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public CheckoutService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        // ── POST /api/checkout ──
        public async Task<CheckoutResponseDto> CreateCheckoutSessionAsync(string userId)
        {
            // 1. جيب الـ Cart
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);

            if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "الـ Cart فاضي — مش ممكن تعمل checkout"
                };

            // 2. بني الـ Line Items لـ Stripe
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cart.CartItems)
            {
                // Stripe بيشتغل بالـ cents (أو أقل وحدة) — ضرب في 100
                var unitAmountInCents = (long)(item.UnitPrice * 100);

                var productName = item.UnitType == UnitType.Weight
                    ? $"{item.ProductName} ({item.WeightInGrams}g)"
                    : item.ProductName;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",                  // ← غيّرها لو محتاج
                        UnitAmount = unitAmountInCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productName,
                            Images = item.ImageUrl != null
                                ? new List<string> { item.ImageUrl }
                                : null
                        }
                    },
                    Quantity = item.UnitType == UnitType.Weight ? 1 : item.Quantity
                });
            }

            // 3. إنشئ الـ Session
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
                    { "userId", userId },
                    { "cartId", cart.Id.ToString() }
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
                Message = "Checkout session created",
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
                    Message = "Status retrieved",
                    SessionId = session.Id,
                    PaymentStatus = session.PaymentStatus,  // paid / unpaid
                    SessionStatus = session.Status          // open / complete / expired
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
