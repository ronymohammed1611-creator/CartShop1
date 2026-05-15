using CartShop.BLL.Dtos;
using System.Threading.Tasks;

namespace CartShop.BLL.Interfaces
{
    public interface ICheckoutService
    {
        /// <summary>
        /// ينشئ Stripe Checkout Session من الـ Cart الحالي للـ user
        /// </summary>
        Task<CheckoutResponseDto> CreateCheckoutSessionAsync(string userId);

        /// <summary>
        /// يجيب حالة الـ payment لـ session معين
        /// </summary>
        Task<CheckoutStatusDto> GetCheckoutStatusAsync(string sessionId);
    }
}
