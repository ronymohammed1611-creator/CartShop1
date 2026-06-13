using CartShop.BLL.Dtos;
using CartShop.BLL.Interfaces;
using CartShop.DAL.Model.Authantication.Login;
using CartShop.DAL.Model.Authantication.Register;
using CartShop.DAL.Model.Authantication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CartShop.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // ── Register ──
        public async Task<AuthResponseDto> RegisterAsync(Register model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };

            if (model.Password != model.ConfirmPassword)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match"
                };

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            // ✅ Cart بيتعمل تلقائياً أول ما يضيف منتج — مش هنا
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = token.Token,
                TokenExpiration = token.Expiration,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        // ── Login بـ FullName ──
        public async Task<AuthResponseDto> LoginAsync(Login model)
        {
            // بندور على اليوزر بالـ FullName بدل الـ Email
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.FullName == model.FullName);

            if (user == null)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid name or password"
                };

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid name or password"
                };

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token.Token,
                TokenExpiration = token.Expiration,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        // ── Logout ──
        // JWT stateless — الـ logout الفعلي على الـ client (بيمسح الـ token)
        public Task<AuthResponseDto> LogoutAsync(string userId)
        {
            return Task.FromResult(new AuthResponseDto
            {
                Success = true,
                Message = "Logged out successfully"
            });
        }

        // ── JWT Generator ──
        private (string Token, DateTime Expiration) GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }
    }
}
