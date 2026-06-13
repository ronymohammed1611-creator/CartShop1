using System.Threading.Tasks;
using CartShop.BLL.Dtos;

namespace CartShop.BLL.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> CreateCheckoutSessionAsync(string userId);

        Task<CheckoutStatusDto> GetCheckoutStatusAsync(string sessionId);
    }
}