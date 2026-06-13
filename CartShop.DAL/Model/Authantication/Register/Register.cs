using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace CartShop.DAL.Model.Authantication.Register
{


    public class Register
    {
        [Required(ErrorMessage = "Please Enter Your Full Name")]
        public string FullName { get; set; }
       
   

        [Required(ErrorMessage = "Please Enter Your Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Valid Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Password Again")]
        public string ConfirmPassword { get; set; }

    }


}
