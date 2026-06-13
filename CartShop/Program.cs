using CartShop.BLL.Interfaces;
using CartShop.BLL.Services;
using CartShop.DAL.Data;
using CartShop.DAL.Model.Authantication;
using CartShop.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System;
using System.Text;
using AppCheckoutService = CartShop.BLL.Services.CheckoutService;

namespace CartShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // STRIPE
            // =========================
            StripeConfiguration.ApiKey =
                builder.Configuration["Stripe:SecretKey"]
                ?? Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                ?? Environment.GetEnvironmentVariable("Stripe__SecretKey");

            // =========================
            // CONNECTION STRING
            // =========================
            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Database connection string is missing!");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.CommandTimeout(120);
                    npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            });

            // =========================
            // IDENTITY
            // =========================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // =========================
            // JWT AUTH (FIXED)
            // =========================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])
                    )
                };

                // مهم جدًا: منع التحويل لـ /Account/Login
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"message\":\"Unauthorized\"}");
                    }
                };
            });

            // =========================
            // CORS
            // =========================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            // =========================
            // DI
            // =========================
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICheckoutService, AppCheckoutService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddControllers();

            // =========================
            // SWAGGER
            // =========================
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CartShop API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            });

            // =========================
            // PORT
            // =========================
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            var app = builder.Build();

            // =========================
            // PIPELINE
            // =========================
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}