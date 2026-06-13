using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.BLL.Dtos
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
