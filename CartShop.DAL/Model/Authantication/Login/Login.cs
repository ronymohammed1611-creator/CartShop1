using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.DAL.Model.Authantication.Login
{
    public class Login
    {
        [Required(ErrorMessage = "Please Enter Your Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please Enter Valid Password")]
        public string Password { get; set; }
    }
}
