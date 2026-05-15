using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model
{
    // منتج واحد ممكن يبقى عليه أكتر من أوفر
    // والأوفر الواحدة ممكن تكون على أكتر من منتج
    public class ProductOffer
    {
        public int ProductId { get; set; }
        public int OfferId { get; set; }

        // Navigation
        public Product Product { get; set; }
        public Offer Offer { get; set; }
    }
}
