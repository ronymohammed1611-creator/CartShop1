using CartShop.DAL.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartShop.DAL.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        /// <summary>
        /// يجيب الـ Order مع الـ OrderItems بتاعته
        /// </summary>
        Task<Order> GetByIdWithItemsAsync(int orderId);

        /// <summary>
        /// كل orders يوزر معين مرتبة من الأحدث
        /// </summary>
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    }
}
