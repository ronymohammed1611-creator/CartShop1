using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model.Enums
{
    public enum AddedBy
    {
        Barcode,    // عن طريق الباركود
        AI,         // عن طريق الكاميرا
        Manual      // يدوي من الشاشة
    }
}
