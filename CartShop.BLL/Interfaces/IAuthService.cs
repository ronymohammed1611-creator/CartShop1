using CartShop.BLL.Dtos;
using CartShop.DAL.Model.Authantication.Login;
using CartShop.DAL.Model.Authantication.Register;
using System.Threading.Tasks;

namespace CartShop.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(Register model);
        Task<AuthResponseDto> LoginAsync(Login model);
        Task<AuthResponseDto> LogoutAsync(string userId);
    }
}
