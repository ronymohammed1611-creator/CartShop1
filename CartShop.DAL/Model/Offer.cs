using CartShop.DAL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
    public class Offer
    {
        public int Id { get; set; }
        public string Title { get; set; }          
        public string? Description { get; set; }
        public OfferType Type { get; set; }         // Percentage أو FixedAmount
        public decimal DiscountValue { get; set; }  // 20 أو 5 جنيه
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<ProductOffer> ProductOffers { get; set; }
    }
}
