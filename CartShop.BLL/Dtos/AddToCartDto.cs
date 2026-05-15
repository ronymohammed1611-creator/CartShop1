using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.BLL.Dtos
{
    public class AddToCartDto
    {
        public string ProductName { get; set; }
        public decimal WeightInGrams { get; set; }
        public string? ImageUrl { get; set; }
        public UnitType UnitType { get; set; }

      
        public decimal? BaseUnitPrice { get; set; }
        public decimal? WeightPrice { get; set; }
    }
}
