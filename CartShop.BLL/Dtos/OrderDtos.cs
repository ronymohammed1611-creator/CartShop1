using System;
using System.Collections.Generic;

namespace CartShop.BLL.Dtos
{
    // Response من GET /api/orders
    public class OrderListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<OrderSummaryDto> Orders { get; set; } = new();
    }

    // Response من GET /api/orders/:orderId
    public class OrderDetailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public OrderDetailDto Order { get; set; }
    }

    // ملخص الـ Order في الـ List
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // تفاصيل الـ Order الكاملة
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string? StripeSessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public string UnitType { get; set; }
        public int Quantity { get; set; }
        public decimal WeightInGrams { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
