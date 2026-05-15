using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model.Enums
{
    public enum RecommendationType
    {
        BasedOnHistory,   // بناءً على مشترياته السابقة
        BasedOnCategory,  // بناءً على نفس الكاتيجوري
        Trending          // منتجات رائجة
    }
}
