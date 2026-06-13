using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.BLL.Dtos
{
    public class CartResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CartDto? Cart { get; set; }
    }

    public class CartDto
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal WeightInGrams { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string UnitType { get; set; }
    }
}
