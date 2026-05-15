using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartShop.DAL.Model.Enums;
using CartShop.DAL.Model.CartShop.DAL.Model;

namespace CartShop.DAL.Model
{
  
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Barcode { get; set; }
            public decimal Price { get; set; }
            public string? ImageUrl { get; set; }
            public string? Description { get; set; }
            public string? Category { get; set; }        // ← عشان الـ Recommendation
            public UnitType UnitType { get; set; }
            public decimal BaseUnitValue { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            // Navigation
            public ICollection<CartItem> CartItems { get; set; }
            public ICollection<OrderItem> OrderItems { get; set; }
            public ICollection<ProductOffer> ProductOffers { get; set; } 
        }
    }
