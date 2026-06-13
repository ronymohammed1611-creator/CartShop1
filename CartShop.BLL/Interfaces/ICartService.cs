using CartShop.BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.BLL.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartResponseDto> GetCartAsync(string userId);
        Task<CartResponseDto> UpdateQuantityAsync(string userId, int cartItemId, int quantity);
        Task<CartResponseDto> RemoveItemAsync(string userId, int cartItemId);
        Task<CartResponseDto> ClearCartAsync(string userId);
    }
}
