using CartShop.DAL.Data;
using CartShop.DAL.Model;
using CartShop.DAL.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartShop.DAL.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart> GetCartWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductOffers)
                            .ThenInclude(po => po.Offer)
                .FirstOrDefaultAsync(c => c.UserId == userId
                                       && c.Status == CartStatus.Active);
        }
    }
}