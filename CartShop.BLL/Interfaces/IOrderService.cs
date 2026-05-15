using CartShop.BLL.Dtos;
using System.Threading.Tasks;

namespace CartShop.BLL.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// كل orders اليوزر
        /// </summary>
        Task<OrderListResponseDto> GetOrdersAsync(string userId);

        /// <summary>
        /// تفاصيل order معين
        /// </summary>
        Task<OrderDetailResponseDto> GetOrderByIdAsync(string userId, int orderId);

        /// <summary>
        /// بيتعمل من الـ Webhook بعد ما Stripe يأكد الدفع
        /// </summary>
        Task<bool> CreateOrderFromCartAsync(string userId, string stripeSessionId);
    }
}
