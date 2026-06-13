using System;
using System.Collections.Generic;

namespace CartShop.BLL.Dtos
{
    // Response من POST /api/checkout
    public class CheckoutResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? SessionId { get; set; }
        public string? CheckoutUrl { get; set; }   // رابط صفحة الدفع Stripe
    }

    // Response من GET /api/checkout/status/:sessionId
    public class CheckoutStatusDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string SessionId { get; set; }
        public string PaymentStatus { get; set; }  // paid / unpaid / no_payment_required
        public string SessionStatus { get; set; }  // open / complete / expired
    }
}
