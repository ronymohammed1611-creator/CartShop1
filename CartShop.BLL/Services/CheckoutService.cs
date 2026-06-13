using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CheckoutResponseDto> CreateCheckoutSessionAsync(string userId, CheckoutRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "User is not authenticated"
                };
            }

            if (request?.Items == null || request.Items.Count == 0)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "Cart is empty"
                };
            }

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in request.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Name) || item.Price <= 0)
                    continue;

                var quantity = item.Quantity > 0 ? item.Quantity : 1;
                var unitAmountInCents = (long)Math.Round(item.Price * 100, MidpointRounding.AwayFromZero);

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
                    Quantity = quantity
                });
            }

            if (lineItems.Count == 0)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = "No valid cart items to checkout"
                };
            }

            var frontendUrl =
                _configuration["Stripe:Frontend:BaseUrl"]
                ?? _configuration["Frontend:BaseUrl"]
                ?? "http://localhost:5173";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{frontendUrl.TrimEnd('/')}/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{frontendUrl.TrimEnd('/')}/payment",
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId },
                    { "source", "firebase_cart" }
                }
            };

            var service = new SessionService();

            try
            {
                var session = await service.CreateAsync(options);

                return new CheckoutResponseDto
                {
                    Success = true,
                    Message = "Checkout session created successfully",
                    SessionId = session.Id,
                    CheckoutUrl = session.Url
                };
            }
            catch (Exception ex)
            {
                return new CheckoutResponseDto
                {
                    Success = false,
                    Message = $"Stripe error: {ex.Message}"
                };
            }
        }

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
