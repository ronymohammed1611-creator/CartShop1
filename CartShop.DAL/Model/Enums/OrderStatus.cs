using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model.Enums
{
    public enum OrderStatus
    {
        Pending,    // في الانتظار
        Confirmed,  // تم التأكيد
        Cancelled   // تم الإلغاء
    }
}
