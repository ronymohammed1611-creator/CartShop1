using CartShop.BLL.Dtos;
using System.Threading.Tasks;

namespace CartShop.BLL.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> CreateCheckoutSessionAsync(string userId, CheckoutRequest request);

        Task<CheckoutStatusDto> GetCheckoutStatusAsync(string sessionId);
    }
}
