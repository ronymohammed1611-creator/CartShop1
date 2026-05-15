using CartShop.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Repositories
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task<Cart> GetCartWithItemsAsync(string userId);
    }

}
