using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
   
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace CartShop.DAL.Model
    {
        public class CartItem
        {
            public int Id { get; set; }
            public int CartId { get; set; }
            public int? ProductId { get; set; }          // ← Optional مش Required
            public string ProductName { get; set; }      // ← جديد
            public string? ImageUrl { get; set; }        // ← جديد
            public int Quantity { get; set; } = 1;
            public decimal WeightInGrams { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public UnitType UnitType { get; set; }       // ← جديد
            public AddedBy AddedBy { get; set; }
            public DateTime AddedAt { get; set; } = DateTime.UtcNow;

            // Navigation
            public Cart Cart { get; set; }
            public Product? Product { get; set; }        // ← Optional زي الـ ProductId
        }
    }
}
