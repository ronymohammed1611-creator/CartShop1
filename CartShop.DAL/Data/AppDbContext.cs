using CartShop.DAL.Model;
using CartShop.DAL.Model.Authantication;
using CartShop.DAL.Model.CartShop.DAL.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CartShop.DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<ProductOffer> ProductOffers { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── ProductOffer (Many-to-Many) ──
            builder.Entity<ProductOffer>()
                .HasKey(po => new { po.ProductId, po.OfferId });

            builder.Entity<ProductOffer>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOffers)
                .HasForeignKey(po => po.ProductId);

            builder.Entity<ProductOffer>()
                .HasOne(po => po.Offer)
                .WithMany(o => o.ProductOffers)
                .HasForeignKey(po => po.OfferId);

            // ── Cart ──
            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            // ── CartItem ──
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId);

            // ── Order ──
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // ── OrderItem ──
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // ── Recommendation ──
            builder.Entity<Recommendation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            builder.Entity<Recommendation>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId);

            // ── Decimal Precision ──
            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            builder.Entity<Product>().Property(p => p.BaseUnitValue).HasPrecision(18, 2);

            builder.Entity<CartItem>().Property(ci => ci.UnitPrice).HasPrecision(18, 2);
            builder.Entity<CartItem>().Property(ci => ci.TotalPrice).HasPrecision(18, 2);
            builder.Entity<CartItem>().Property(ci => ci.WeightInGrams).HasPrecision(18, 2);

            builder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.TotalPrice).HasPrecision(18, 2);

            builder.Entity<Offer>().Property(o => o.DiscountValue).HasPrecision(18, 2);
            builder.Entity<Recommendation>().Property(r => r.Score).HasPrecision(18, 2);

            // ❌ IMPORTANT: removed HasData seed (سبب كل المشاكل)
        }
    }
}