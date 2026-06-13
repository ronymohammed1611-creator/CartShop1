using CartShop.DAL.Model.Authantication;
using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
    public class Recommendation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public RecommendationType Type { get; set; }
        public decimal Score { get; set; }        // نسبة التوصية
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}
