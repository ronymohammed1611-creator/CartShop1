using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using CartShop.DAL.Model;
using CartShop.DAL.Model.Enums;
using CartShop.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartShop.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── GET /api/orders ── كل orders اليوزر
        public async Task<OrderListResponseDto> GetOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);

            var summaries = orders.Select(o => new OrderSummaryDto
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                CreatedAt = o.CreatedAt
            }).ToList();

            return new OrderListResponseDto
            {
                Success = true,
                Message = $"{summaries.Count} order(s) found",
                Orders = summaries
            };
        }

        // ── GET /api/orders/:orderId ── تفاصيل order معين
        public async Task<OrderDetailResponseDto> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(orderId);

            if (order == null || order.UserId != userId)
                return new OrderDetailResponseDto
                {
                    Success = false,
                    Message = "Order مش موجود أو مش تبعك"
                };

            var dto = new OrderDetailDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StripeSessionId = order.StripeSessionId,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductName = oi.ProductName,
                    ImageUrl = oi.ImageUrl,
                    UnitType = oi.UnitType,
                    Quantity = oi.Quantity,
                    WeightInGrams = oi.WeightInGrams,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList() ?? new List<OrderItemDto>()
            };

            return new OrderDetailResponseDto
            {
                Success = true,
                Order = dto
            };
        }

        // ── Webhook — ينشئ Order من الـ Cart بعد تأكيد Stripe ──
        public async Task<bool> CreateOrderFromCartAsync(string userId, string stripeSessionId)
        {
            try
            {
                // 1. جيب الـ Cart
                var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    return false;

                // 2. إنشئ الـ Order
                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = cart.CartItems.Sum(ci => ci.TotalPrice),
                    Status = OrderStatus.Confirmed,
                    PaymentStatus = PaymentStatus.Paid,
                    StripeSessionId = stripeSessionId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // 3. إنشئ الـ OrderItems من الـ CartItems
                foreach (var ci in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductName = ci.ProductName,
                        ImageUrl = ci.ImageUrl,
                        UnitType = ci.UnitType.ToString(),
                        Quantity = ci.Quantity,
                        WeightInGrams = ci.WeightInGrams,
                        UnitPrice = ci.UnitPrice,
                        TotalPrice = ci.TotalPrice
                    };
                    await _unitOfWork.OrderItems.AddAsync(orderItem);
                }

                // 4. امسح الـ Cart وغيّر status لـ Completed
                cart.CartItems.Clear();
                cart.Status = CartStatus.CheckedOut;
                _unitOfWork.Carts.Update(cart);

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
