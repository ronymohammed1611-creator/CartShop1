using System.Collections.Generic;

namespace CartShop.BLL.Dtos
{
    public class CheckoutRequest
    {
        public List<CheckoutItemDto> Items { get; set; } = new();
        public decimal? Total { get; set; }
    }

    public class CheckoutItemDto
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
