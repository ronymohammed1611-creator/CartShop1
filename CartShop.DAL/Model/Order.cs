using CartShop.DAL.Model.Authantication;
using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Stripe
        public string? StripePaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Navigation
        public ApplicationUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
