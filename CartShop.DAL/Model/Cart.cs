using CartShop.DAL.Model.Authantication;
using CartShop.DAL.Model.CartShop.DAL.Model;
using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CartStatus Status { get; set; } = CartStatus.Active;

        // Navigation
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

   
}
